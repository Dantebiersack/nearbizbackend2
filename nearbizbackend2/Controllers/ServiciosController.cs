using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using nearbizbackend.Data;
using nearbizbackend.DTOs;
using nearbizbackend.Models;

namespace nearbizbackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ServiciosController : ControllerBase
    {
        private readonly NearBizDbContext _db;
        public ServiciosController(NearBizDbContext db) => _db = db;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ServicioReadDto>>> GetAll([FromQuery] bool includeInactive = false)
        {
            var q = includeInactive ? _db.Servicios.IgnoreQueryFilters() : _db.Servicios;
            var items = await q.AsNoTracking()
                .Select(s => new ServicioReadDto(s.IdServicio, s.IdNegocio, s.NombreServicio, s.Descripcion, s.Precio, s.DuracionMinutos, s.Estado))
                .ToListAsync();
            return Ok(items);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<ServicioReadDto>> Get(int id)
        {
            var s = await _db.Servicios.IgnoreQueryFilters().FirstOrDefaultAsync(x => x.IdServicio == id);
            if (s is null) return NotFound();
            return Ok(new ServicioReadDto(s.IdServicio, s.IdNegocio, s.NombreServicio, s.Descripcion, s.Precio, s.DuracionMinutos, s.Estado));
        }

        [HttpPost]
        public async Task<ActionResult<ServicioReadDto>> Create(ServicioCreateDto dto)
        {
            var e = new Servicio
            {
                IdNegocio = dto.IdNegocio,
                NombreServicio = dto.NombreServicio,
                Descripcion = dto.Descripcion,
                Precio = dto.Precio,
                DuracionMinutos = dto.DuracionMinutos,
                Estado = true
            };
            _db.Servicios.Add(e);
            await _db.SaveChangesAsync();
            return CreatedAtAction(nameof(Get), new { id = e.IdServicio },
                new ServicioReadDto(e.IdServicio, e.IdNegocio, e.NombreServicio, e.Descripcion, e.Precio, e.DuracionMinutos, e.Estado));
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, ServicioUpdateDto dto)
        {
            var e = await _db.Servicios.IgnoreQueryFilters().FirstOrDefaultAsync(x => x.IdServicio == id);
            if (e is null) return NotFound();
            e.IdNegocio = dto.IdNegocio;
            e.NombreServicio = dto.NombreServicio;
            e.Descripcion = dto.Descripcion;
            e.Precio = dto.Precio;
            e.DuracionMinutos = dto.DuracionMinutos;
            await _db.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> SoftDelete(int id)
        {
            var e = await _db.Servicios.IgnoreQueryFilters().FirstOrDefaultAsync(x => x.IdServicio == id);
            if (e is null) return NotFound();
            e.Estado = false;
            await _db.SaveChangesAsync();
            return NoContent();
        }

        [HttpPatch("{id:int}/restore")]
        public async Task<IActionResult> Restore(int id)
        {
            var e = await _db.Servicios.IgnoreQueryFilters().FirstOrDefaultAsync(x => x.IdServicio == id);
            if (e is null) return NotFound();
            e.Estado = true;
            await _db.SaveChangesAsync();
            return NoContent();
        }
    }
}
