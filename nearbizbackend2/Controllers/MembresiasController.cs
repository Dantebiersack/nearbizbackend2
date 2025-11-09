using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using nearbizbackend.Data;
using nearbizbackend.DTOs;
using nearbizbackend.Models;

namespace nearbizbackend2.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MembresiasController : ControllerBase
    {
        private readonly NearBizDbContext _db;
        public MembresiasController(NearBizDbContext db) => _db = db;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<MembresiaReadDto>>> GetAll([FromQuery] bool includeInactive = false)
        {
            var q = includeInactive ? _db.Membresias.IgnoreQueryFilters() : _db.Membresias;
            var items = await q.AsNoTracking()
                .Select(m => new MembresiaReadDto(
                    m.IdMembresia,
                    m.PrecioMensual,
                    m.IdNegocio,
                    m.Estado,
                    m.UltimaRenovacion
                ))
                .ToListAsync();
            return Ok(items);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<MembresiaReadDto>> Get(int id)
        {
            var m = await _db.Membresias.IgnoreQueryFilters()
                        .FirstOrDefaultAsync(x => x.IdMembresia == id);
            if (m is null) return NotFound();

            return Ok(new MembresiaReadDto(
                m.IdMembresia,
                m.PrecioMensual,
                m.IdNegocio,
                m.Estado,
                m.UltimaRenovacion
            ));
        }

        [HttpPost]
        public async Task<ActionResult<MembresiaReadDto>> Create(MembresiaCreateDto dto)
        {
            var e = new Membresia
            {
                PrecioMensual = dto.PrecioMensual,
                IdNegocio = dto.IdNegocio,
                Estado = true,
                UltimaRenovacion = dto.UltimaRenovacion ?? DateTime.UtcNow 
            };

            _db.Membresias.Add(e);
            await _db.SaveChangesAsync();

            return CreatedAtAction(nameof(Get), new { id = e.IdMembresia },
                new MembresiaReadDto(
                    e.IdMembresia,
                    e.PrecioMensual,
                    e.IdNegocio,
                    e.Estado,
                    e.UltimaRenovacion
                ));
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, MembresiaUpdateDto dto)
        {
            var e = await _db.Membresias.IgnoreQueryFilters()
                        .FirstOrDefaultAsync(x => x.IdMembresia == id);
            if (e is null) return NotFound();

            e.PrecioMensual = dto.PrecioMensual;
            e.IdNegocio = dto.IdNegocio;

            if (dto.UltimaRenovacion.HasValue)
                e.UltimaRenovacion = dto.UltimaRenovacion.Value;

            await _db.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> SoftDelete(int id)
        {
            var e = await _db.Membresias.IgnoreQueryFilters()
                        .FirstOrDefaultAsync(x => x.IdMembresia == id);
            if (e is null) return NotFound();

            e.Estado = false;
            await _db.SaveChangesAsync();
            return NoContent();
        }

        [HttpPatch("{id:int}/restore")]
        public async Task<IActionResult> Restore(int id)
        {
            var e = await _db.Membresias.IgnoreQueryFilters()
                        .FirstOrDefaultAsync(x => x.IdMembresia == id);
            if (e is null) return NotFound();

            e.Estado = true;
            await _db.SaveChangesAsync();
            return NoContent();
        }

        // NUEVO: renovar (marca fecha de última renovación en UTC y activa si estaba inactiva)
        [HttpPatch("{id:int}/renew")]
        public async Task<IActionResult> Renew(int id)
        {
            var e = await _db.Membresias.IgnoreQueryFilters()
                        .FirstOrDefaultAsync(x => x.IdMembresia == id);
            if (e is null) return NotFound();

            e.UltimaRenovacion = DateTime.UtcNow;
            e.Estado = true;
            await _db.SaveChangesAsync();

            return NoContent();
        }

        [HttpGet("admin")]
        public async Task<ActionResult<IEnumerable<MembresiaAdminRowDto>>> GetGrid([FromQuery] bool includeInactive = true)
        {
            var mset = includeInactive ? _db.Membresias.IgnoreQueryFilters() : _db.Membresias;
            var nset = includeInactive ? _db.Negocios.IgnoreQueryFilters() : _db.Negocios;

            var rows = await mset.AsNoTracking()
                .Join(nset.AsNoTracking(),
                      m => m.IdNegocio,
                      n => n.IdNegocio,
                      (m, n) => new MembresiaAdminRowDto(
                          m.IdMembresia,
                          m.IdNegocio,
                          n.Nombre,
                          m.PrecioMensual,
                          m.Estado,
                          m.UltimaRenovacion 
                      ))
                .ToListAsync();

            return Ok(rows);
        }

        public record MembresiaCreateForBusinessDto(decimal? PrecioMensual);

        [HttpPost("create-for-business/{idNegocio:int}")]
        public async Task<ActionResult<MembresiaReadDto>> CreateForBusiness(int idNegocio, MembresiaCreateForBusinessDto dto)
        {
            // 1) Validar que exista el negocio
            var negocio = await _db.Negocios.IgnoreQueryFilters()
                            .FirstOrDefaultAsync(n => n.IdNegocio == idNegocio);
            if (negocio is null)
                return NotFound(new { message = "Negocio no encontrado" });

            // 2) Validar que NO tenga membresía (suponiendo 1 a 1)
            var yaTiene = await _db.Membresias.IgnoreQueryFilters()
                            .AnyAsync(m => m.IdNegocio == idNegocio);
            if (yaTiene)
                return Conflict(new { message = "El negocio ya tiene membresía" });

            // 3) Crear
            var e = new Membresia
            {
                IdNegocio = idNegocio,
                PrecioMensual = dto.PrecioMensual ?? 0m,
                Estado = true,
                UltimaRenovacion = DateTime.UtcNow, 
            };
            _db.Membresias.Add(e);
            await _db.SaveChangesAsync();

            return Ok(new MembresiaReadDto(
                e.IdMembresia,
                e.PrecioMensual,
                e.IdNegocio,
                e.Estado,
                e.UltimaRenovacion
            ));
        }

    }
}
