using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using nearbizbackend.Data;
using nearbizbackend.DTOs;
using nearbizbackend.Models;

namespace nearbizbackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ValoracionesController : ControllerBase
    {
        private readonly NearBizDbContext _db;
        public ValoracionesController(NearBizDbContext db) => _db = db;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ValoracionReadDto>>> GetAll([FromQuery] bool includeInactive = false)
        {
            var q = includeInactive ? _db.Valoraciones.IgnoreQueryFilters() : _db.Valoraciones;
            var items = await q.AsNoTracking()
                .Select(v => new ValoracionReadDto(v.IdValoracion, v.IdCita, v.IdCliente, v.IdNegocio, v.Calificacion, v.Comentario, v.Fecha, v.Estado))
                .ToListAsync();
            return Ok(items);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<ValoracionReadDto>> Get(int id)
        {
            var v = await _db.Valoraciones.IgnoreQueryFilters().FirstOrDefaultAsync(x => x.IdValoracion == id);
            if (v is null) return NotFound();
            return Ok(new ValoracionReadDto(v.IdValoracion, v.IdCita, v.IdCliente, v.IdNegocio, v.Calificacion, v.Comentario, v.Fecha, v.Estado));
        }

        [HttpPost]
        public async Task<ActionResult<ValoracionReadDto>> Create(ValoracionCreateDto dto)
        {
            var e = new Valoracion
            {
                IdCita = dto.IdCita,
                IdCliente = dto.IdCliente,
                IdNegocio = dto.IdNegocio,
                Calificacion = dto.Calificacion,
                Comentario = dto.Comentario,
                Estado = true
            };
            _db.Valoraciones.Add(e);
            await _db.SaveChangesAsync();
            return CreatedAtAction(nameof(Get), new { id = e.IdValoracion },
                new ValoracionReadDto(e.IdValoracion, e.IdCita, e.IdCliente, e.IdNegocio, e.Calificacion, e.Comentario, e.Fecha, e.Estado));
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, ValoracionUpdateDto dto)
        {
            var e = await _db.Valoraciones.IgnoreQueryFilters().FirstOrDefaultAsync(x => x.IdValoracion == id);
            if (e is null) return NotFound();
            e.Calificacion = dto.Calificacion;
            e.Comentario = dto.Comentario;
            await _db.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> SoftDelete(int id)
        {
            var e = await _db.Valoraciones.IgnoreQueryFilters().FirstOrDefaultAsync(x => x.IdValoracion == id);
            if (e is null) return NotFound();
            e.Estado = false;
            await _db.SaveChangesAsync();
            return NoContent();
        }

        [HttpPatch("{id:int}/restore")]
        public async Task<IActionResult> Restore(int id)
        {
            var e = await _db.Valoraciones.IgnoreQueryFilters().FirstOrDefaultAsync(x => x.IdValoracion == id);
            if (e is null) return NotFound();
            e.Estado = true;
            await _db.SaveChangesAsync();
            return NoContent();
        }
    }
}
