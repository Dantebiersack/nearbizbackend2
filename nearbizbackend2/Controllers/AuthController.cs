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
        public async Task<ActionResult<LoginResponse>> Login(LoginRequest req)
        {
            // 1. Buscar usuario por nombre o email
            var user = await _db.Usuarios
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(u =>
                    (u.Email == req.UserOrEmail || u.Nombre == req.UserOrEmail) &&
                    u.Estado == true);

            if (user is null)
                return Unauthorized("Usuario o contraseña inválidos");

            // 2. Validar contraseña
            // TODO: aquí debería ir el hash. Por ahora plain:
            if (user.ContrasenaHash != req.Password)
                return Unauthorized("Usuario o contraseña inválidos");

            // 3. Obtener rol
            var rol = await _db.Roles
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.IdRol == user.IdRol);

            var rolNombre = rol?.RolNombre ?? "negocio";

            // 4. Generar token
            var token = GenerateJwt(user.IdUsuario, user.Nombre, rolNombre);

            // 5. Guardar token en BD (opcional)
            user.Token = token;
            await _db.SaveChangesAsync();

            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(token);

            return Ok(new LoginResponse(
                token,
                jwt.ValidTo,
                user.Nombre,
                user.IdUsuario,
                rolNombre,
                user.Email
            ));
        }

        private string GenerateJwt(int idUsuario, string nombre, string rol)
        {
            var jwtSection = _config.GetSection("Jwt");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSection["Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, idUsuario.ToString()),
                new Claim(JwtRegisteredClaimNames.UniqueName, nombre),
                new Claim(ClaimTypes.Role, rol)
            };

            var expires = DateTime.UtcNow.AddDays(1); // 1 día

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
