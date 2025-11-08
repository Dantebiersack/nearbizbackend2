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
                .Select(m => new MembresiaReadDto(m.IdMembresia, m.PrecioMensual, m.IdNegocio, m.Estado))
                .ToListAsync();
            return Ok(items);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<MembresiaReadDto>> Get(int id)
        {
            var m = await _db.Membresias.IgnoreQueryFilters().FirstOrDefaultAsync(x => x.IdMembresia == id);
            if (m is null) return NotFound();
            return Ok(new MembresiaReadDto(m.IdMembresia, m.PrecioMensual, m.IdNegocio, m.Estado));
        }

        [HttpPost]
        public async Task<ActionResult<MembresiaReadDto>> Create(MembresiaCreateDto dto)
        {
            var e = new Membresia { PrecioMensual = dto.PrecioMensual, IdNegocio = dto.IdNegocio, Estado = true };
            _db.Membresias.Add(e);
            await _db.SaveChangesAsync();
            return CreatedAtAction(nameof(Get), new { id = e.IdMembresia },
                new MembresiaReadDto(e.IdMembresia, e.PrecioMensual, e.IdNegocio, e.Estado));
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, MembresiaUpdateDto dto)
        {
            var e = await _db.Membresias.IgnoreQueryFilters().FirstOrDefaultAsync(x => x.IdMembresia == id);
            if (e is null) return NotFound();
            e.PrecioMensual = dto.PrecioMensual;
            e.IdNegocio = dto.IdNegocio;
            await _db.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> SoftDelete(int id)
        {
            var e = await _db.Membresias.IgnoreQueryFilters().FirstOrDefaultAsync(x => x.IdMembresia == id);
            if (e is null) return NotFound();
            e.Estado = false;
            await _db.SaveChangesAsync();
            return NoContent();
        }

        [HttpPatch("{id:int}/restore")]
        public async Task<IActionResult> Restore(int id)
        {
            var e = await _db.Membresias.IgnoreQueryFilters().FirstOrDefaultAsync(x => x.IdMembresia == id);
            if (e is null) return NotFound();
            e.Estado = true;
            await _db.SaveChangesAsync();
            return NoContent();
        }
    }
}
