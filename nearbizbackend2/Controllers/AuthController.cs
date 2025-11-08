using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using nearbizbackend.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace nearbizbackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [AllowAnonymous]
    public class AuthController : ControllerBase
    {
        private readonly NearBizDbContext _db;
        private readonly IConfiguration _config;

        public AuthController(NearBizDbContext db, IConfiguration config)
        {
            _db = db;
            _config = config;
        }

        public record LoginRequest(string UserOrEmail, string Password);
        public record LoginResponse(string Token, DateTime Expira, string Nombre, int IdUsuario, string Rol, string Email);

        [HttpPost("login")]
        [Consumes("application/json")]
        [Produces("application/json")]
        public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest? req)
        {
            try
            {
                if (req is null || string.IsNullOrWhiteSpace(req.UserOrEmail) || string.IsNullOrWhiteSpace(req.Password))
                    return BadRequest(new { message = "Body inválido. Envía JSON { userOrEmail, password }" });

                // 1) Buscar usuario
                var user = await _db.Usuarios
                    .IgnoreQueryFilters()
                    .FirstOrDefaultAsync(u =>
                        (u.Email == req.UserOrEmail || u.Nombre == req.UserOrEmail) &&
                        u.Estado == true);

                if (user is null)
                    return Unauthorized(new { message = "Usuario o contraseña inválidos" });

                // 2) Validar contraseña (por ahora en plano)
                if (user.ContrasenaHash != req.Password)
                    return Unauthorized(new { message = "Usuario o contraseña inválidos" });

                // 3) Rol
                var rolNombre = await _db.Roles
                    .Where(r => r.IdRol == user.IdRol)
                    .Select(r => r.RolNombre)
                    .FirstOrDefaultAsync() ?? "negocio";

                // 4) JWT
                var token = GenerateJwt(user.IdUsuario, user.Nombre, rolNombre);
                var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);

                // 5) Guardar token (opcional)
                user.Token = token;
                await _db.SaveChangesAsync();

                return Ok(new LoginResponse(
                    token, jwt.ValidTo, user.Nombre, user.IdUsuario, rolNombre, user.Email));
            }
            catch (Exception ex)
            {
                // mientras depuras, te devuelve el motivo exacto del 500
                return Problem(title: "Login error", detail: ex.ToString(), statusCode: 500);
            }
        }

        private string GenerateJwt(int idUsuario, string nombre, string rol)
        {
            var jwtSection = _config.GetSection("Jwt");
            var keyStr = jwtSection["Key"] ?? throw new InvalidOperationException("Jwt:Key faltante");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyStr));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, idUsuario.ToString()),
                new Claim(JwtRegisteredClaimNames.UniqueName, nombre),
                new Claim(ClaimTypes.Role, rol)
            };

            var expires = DateTime.UtcNow.AddDays(1);

            var token = new JwtSecurityToken(
                issuer: jwtSection["Issuer"],
                audience: jwtSection["Audience"],
                claims: claims,
                expires: expires,
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
