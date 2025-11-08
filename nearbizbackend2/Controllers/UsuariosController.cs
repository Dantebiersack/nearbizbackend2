using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using nearbizbackend.Data;
using nearbizbackend.DTOs;
using nearbizbackend.Models;

namespace nearbizbackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsuariosController : ControllerBase
    {
        private readonly NearBizDbContext _db;
        public UsuariosController(NearBizDbContext db) => _db = db;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<UsuarioReadDto>>> GetAll([FromQuery] bool includeInactive = false)
        {
            var q = includeInactive ? _db.Usuarios.IgnoreQueryFilters() : _db.Usuarios;
            var items = await q.AsNoTracking()
                .Select(x => new UsuarioReadDto(x.IdUsuario, x.Nombre, x.Email, x.IdRol, x.FechaRegistro, x.Estado, x.Token))
                .ToListAsync();
            return Ok(items);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<UsuarioReadDto>> Get(int id)
        {
            var x = await _db.Usuarios.IgnoreQueryFilters().FirstOrDefaultAsync(r => r.IdUsuario == id);
            if (x is null) return NotFound();
            return Ok(new UsuarioReadDto(x.IdUsuario, x.Nombre, x.Email, x.IdRol, x.FechaRegistro, x.Estado, x.Token));
        }

        [HttpPost]
        public async Task<ActionResult<UsuarioReadDto>> Create(UsuarioCreateDto dto)
        {
            var e = new Usuario
            {
                Nombre = dto.Nombre,
                Email = dto.Email,
                ContrasenaHash = dto.ContrasenaHash,
                IdRol = dto.IdRol,
                Token = dto.Token,
                Estado = true
            };
            _db.Usuarios.Add(e);
            await _db.SaveChangesAsync();
            return CreatedAtAction(nameof(Get), new { id = e.IdUsuario },
                new UsuarioReadDto(e.IdUsuario, e.Nombre, e.Email, e.IdRol, e.FechaRegistro, e.Estado, e.Token));
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, UsuarioUpdateDto dto)
        {
            var e = await _db.Usuarios.IgnoreQueryFilters().FirstOrDefaultAsync(x => x.IdUsuario == id);
            if (e is null) return NotFound();
            e.Nombre = dto.Nombre;
            e.Email = dto.Email;
            e.IdRol = dto.IdRol;
            e.Token = dto.Token;
            await _db.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> SoftDelete(int id)
        {
            var e = await _db.Usuarios.IgnoreQueryFilters().FirstOrDefaultAsync(x => x.IdUsuario == id);
            if (e is null) return NotFound();
            e.Estado = false;
            await _db.SaveChangesAsync();
            return NoContent();
        }

        [HttpPatch("{id:int}/restore")]
        public async Task<IActionResult> Restore(int id)
        {
            var e = await _db.Usuarios.IgnoreQueryFilters().FirstOrDefaultAsync(x => x.IdUsuario == id);
            if (e is null) return NotFound();
            e.Estado = true;
            await _db.SaveChangesAsync();
            return NoContent();
        }
    }
}
