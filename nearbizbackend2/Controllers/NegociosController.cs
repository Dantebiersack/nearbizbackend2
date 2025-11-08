using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using nearbizbackend.Data;
using nearbizbackend.DTOs;
using nearbizbackend.Models;

namespace nearbizbackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NegociosController : ControllerBase
    {
        private readonly NearBizDbContext _db;
        public NegociosController(NearBizDbContext db) => _db = db;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<NegocioReadDto>>> GetAll([FromQuery] bool includeInactive = false)
        {
            var q = includeInactive ? _db.Negocios.IgnoreQueryFilters() : _db.Negocios;
            var items = await q.AsNoTracking()
                .Select(n => new NegocioReadDto(
                    n.IdNegocio, n.IdCategoria, n.IdMembresia, n.Nombre, n.Direccion,
                    n.CoordenadasLat, n.CoordenadasLng, n.Descripcion, n.TelefonoContacto,
                    n.CorreoContacto, n.HorarioAtencion, n.Estado, n.LinkUrl))
                .ToListAsync();
            return Ok(items);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<NegocioReadDto>> Get(int id)
        {
            var n = await _db.Negocios.IgnoreQueryFilters().FirstOrDefaultAsync(x => x.IdNegocio == id);
            if (n is null) return NotFound();
            return Ok(new NegocioReadDto(
                n.IdNegocio, n.IdCategoria, n.IdMembresia, n.Nombre, n.Direccion,
                n.CoordenadasLat, n.CoordenadasLng, n.Descripcion, n.TelefonoContacto,
                n.CorreoContacto, n.HorarioAtencion, n.Estado, n.LinkUrl));
        }

        [HttpPost]
        public async Task<ActionResult<NegocioReadDto>> Create(NegocioCreateDto dto)
        {
            var e = new Negocio
            {
                IdCategoria = dto.IdCategoria,
                IdMembresia = dto.IdMembresia,
                Nombre = dto.Nombre,
                Direccion = dto.Direccion,
                CoordenadasLat = dto.CoordenadasLat,
                CoordenadasLng = dto.CoordenadasLng,
                Descripcion = dto.Descripcion,
                TelefonoContacto = dto.TelefonoContacto,
                CorreoContacto = dto.CorreoContacto,
                HorarioAtencion = dto.HorarioAtencion,
                LinkUrl = dto.LinkUrl,
                Estado = true
            };
            _db.Negocios.Add(e);
            await _db.SaveChangesAsync();

            return CreatedAtAction(nameof(Get), new { id = e.IdNegocio },
                new NegocioReadDto(
                    e.IdNegocio, e.IdCategoria, e.IdMembresia, e.Nombre, e.Direccion,
                    e.CoordenadasLat, e.CoordenadasLng, e.Descripcion, e.TelefonoContacto,
                    e.CorreoContacto, e.HorarioAtencion, e.Estado, e.LinkUrl));
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, NegocioUpdateDto dto)
        {
            var e = await _db.Negocios.IgnoreQueryFilters().FirstOrDefaultAsync(x => x.IdNegocio == id);
            if (e is null) return NotFound();

            e.IdCategoria = dto.IdCategoria;
            e.IdMembresia = dto.IdMembresia;
            e.Nombre = dto.Nombre;
            e.Direccion = dto.Direccion;
            e.CoordenadasLat = dto.CoordenadasLat;
            e.CoordenadasLng = dto.CoordenadasLng;
            e.Descripcion = dto.Descripcion;
            e.TelefonoContacto = dto.TelefonoContacto;
            e.CorreoContacto = dto.CorreoContacto;
            e.HorarioAtencion = dto.HorarioAtencion;
            e.LinkUrl = dto.LinkUrl;

            await _db.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> SoftDelete(int id)
        {
            var e = await _db.Negocios.IgnoreQueryFilters().FirstOrDefaultAsync(x => x.IdNegocio == id);
            if (e is null) return NotFound();
            e.Estado = false;
            await _db.SaveChangesAsync();
            return NoContent();
        }

        [HttpPatch("{id:int}/restore")]
        public async Task<IActionResult> Restore(int id)
        {
            var e = await _db.Negocios.IgnoreQueryFilters().FirstOrDefaultAsync(x => x.IdNegocio == id);
            if (e is null) return NotFound();
            e.Estado = true;
            await _db.SaveChangesAsync();
            return NoContent();
        }
    }
}
