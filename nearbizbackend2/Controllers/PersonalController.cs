
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using nearbizbackend.Data;
using nearbizbackend.DTOs;
using nearbizbackend.Models;
using System.Linq;

namespace nearbizbackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PersonalController : ControllerBase
    {
        private readonly NearBizDbContext _db;
        public PersonalController(NearBizDbContext db) => _db = db;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PersonalReadDto>>> GetAll(
            [FromQuery] bool includeInactive = false,
            [FromQuery] int? idNegocio = null //  FILTRO
        )
        {
            var q = includeInactive ? _db.Personal.IgnoreQueryFilters() : _db.Personal;

            if (idNegocio.HasValue)
            {
                q = q.Where(p => p.IdNegocio == idNegocio.Value);
            }
            var items = await q.AsNoTracking()
                .Select(p => new PersonalReadDto(p.IdPersonal, p.IdUsuario, p.IdNegocio, p.RolEnNegocio, p.FechaRegistro, p.Estado))
                .ToListAsync();
            return Ok(items);
        }



        [HttpGet("{id:int}")]
        public async Task<ActionResult<PersonalReadDto>> Get(int id)
        {

            var p = await _db.Personal.IgnoreQueryFilters().FirstOrDefaultAsync(x => x.IdPersonal == id);
            if (p is null) return NotFound();
            return Ok(new PersonalReadDto(p.IdPersonal, p.IdUsuario, p.IdNegocio, p.RolEnNegocio, p.FechaRegistro, p.Estado));
        }

        [HttpPost]
        public async Task<ActionResult<PersonalReadDto>> Create(PersonalCreateDto dto)
        {

            var e = new Personal { IdUsuario = dto.IdUsuario, IdNegocio = dto.IdNegocio, RolEnNegocio = dto.RolEnNegocio, Estado = true };
            _db.Personal.Add(e);
            await _db.SaveChangesAsync();
            return CreatedAtAction(nameof(Get), new { id = e.IdPersonal },
                new PersonalReadDto(e.IdPersonal, e.IdUsuario, e.IdNegocio, e.RolEnNegocio, e.FechaRegistro, e.Estado));
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, PersonalUpdateDto dto)
        {

            var e = await _db.Personal.IgnoreQueryFilters().FirstOrDefaultAsync(x => x.IdPersonal == id);
            if (e is null) return NotFound();
            e.IdUsuario = dto.IdUsuario;
            e.IdNegocio = dto.IdNegocio;
            e.RolEnNegocio = dto.RolEnNegocio;
            await _db.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> SoftDelete(int id)
        {

            var e = await _db.Personal.IgnoreQueryFilters().FirstOrDefaultAsync(x => x.IdPersonal == id);
            if (e is null) return NotFound();
            e.Estado = false;
            var u = await _db.Usuarios.FindAsync(e.IdUsuario);
            if (u != null) u.Estado = false;
            await _db.SaveChangesAsync();
            return NoContent();
        }

        [HttpPatch("{id:int}/restore")]
        public async Task<IActionResult> Restore(int id)
        {
            var e = await _db.Personal.IgnoreQueryFilters().FirstOrDefaultAsync(x => x.IdPersonal == id);
            if (e is null) return NotFound();
            e.Estado = true;
            var u = await _db.Usuarios.IgnoreQueryFilters().FirstOrDefaultAsync(us => us.IdUsuario == e.IdUsuario);
            if (u != null) u.Estado = true;

            await _db.SaveChangesAsync();
            return NoContent();
        }
    }
}