using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using nearbizbackend.Data;
using nearbizbackend.DTOs;
using nearbizbackend.Models;

namespace nearbizbackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PromocionesController : ControllerBase
    {
        private readonly NearBizDbContext _db;
        public PromocionesController(NearBizDbContext db) => _db = db;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PromocionReadDto>>> GetAll([FromQuery] bool includeInactive = false)
        {
            var q = includeInactive ? _db.Promociones.IgnoreQueryFilters() : _db.Promociones;
            var items = await q.AsNoTracking()
                .Select(p => new PromocionReadDto(p.IdPromocion, p.IdNegocio, p.Titulo, p.Descripcion, p.FechaInicio, p.FechaFin, p.Estado))
                .ToListAsync();
            return Ok(items);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<PromocionReadDto>> Get(int id)
        {
            var p = await _db.Promociones.IgnoreQueryFilters().FirstOrDefaultAsync(x => x.IdPromocion == id);
            if (p is null) return NotFound();
            return Ok(new PromocionReadDto(p.IdPromocion, p.IdNegocio, p.Titulo, p.Descripcion, p.FechaInicio, p.FechaFin, p.Estado));
        }

        [HttpPost]
        public async Task<ActionResult<PromocionReadDto>> Create(PromocionCreateDto dto)
        {
            var e = new Promocion
            {
                IdNegocio = dto.IdNegocio,
                Titulo = dto.Titulo,
                Descripcion = dto.Descripcion,
                FechaInicio = dto.FechaInicio,
                FechaFin = dto.FechaFin,
                Estado = true
            };
            _db.Promociones.Add(e);
            await _db.SaveChangesAsync();
            return CreatedAtAction(nameof(Get), new { id = e.IdPromocion },
                new PromocionReadDto(e.IdPromocion, e.IdNegocio, e.Titulo, e.Descripcion, e.FechaInicio, e.FechaFin, e.Estado));
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, PromocionUpdateDto dto)
        {
            var e = await _db.Promociones.IgnoreQueryFilters().FirstOrDefaultAsync(x => x.IdPromocion == id);
            if (e is null) return NotFound();
            e.IdNegocio = dto.IdNegocio;
            e.Titulo = dto.Titulo;
            e.Descripcion = dto.Descripcion;
            e.FechaInicio = dto.FechaInicio;
            e.FechaFin = dto.FechaFin;
            await _db.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> SoftDelete(int id)
        {
            var e = await _db.Promociones.IgnoreQueryFilters().FirstOrDefaultAsync(x => x.IdPromocion == id);
            if (e is null) return NotFound();
            e.Estado = false;
            await _db.SaveChangesAsync();
            return NoContent();
        }

        [HttpPatch("{id:int}/restore")]
        public async Task<IActionResult> Restore(int id)
        {
            var e = await _db.Promociones.IgnoreQueryFilters().FirstOrDefaultAsync(x => x.IdPromocion == id);
            if (e is null) return NotFound();
            e.Estado = true;
            await _db.SaveChangesAsync();
            return NoContent();
        }
    }
}