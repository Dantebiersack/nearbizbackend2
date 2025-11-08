using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using nearbizbackend.Data;
using nearbizbackend.DTOs;
using nearbizbackend.Models;

namespace nearbizbackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CitasController : ControllerBase
    {
        private readonly NearBizDbContext _db;
        public CitasController(NearBizDbContext db) => _db = db;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CitaReadDto>>> GetAll()
        {
            var items = await _db.Citas.AsNoTracking()
                .Select(x => new CitaReadDto(
                    x.IdCita, x.IdCliente, x.IdTecnico, x.IdServicio,
                    x.FechaCita, x.HoraInicio, x.HoraFin, x.Estado, x.MotivoCancelacion,
                    x.FechaCreacion, x.FechaActualizacion))
                .ToListAsync();
            return Ok(items);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<CitaReadDto>> Get(int id)
        {
            var x = await _db.Citas.AsNoTracking().FirstOrDefaultAsync(r => r.IdCita == id);
            if (x is null) return NotFound();
            return Ok(new CitaReadDto(
                x.IdCita, x.IdCliente, x.IdTecnico, x.IdServicio,
                x.FechaCita, x.HoraInicio, x.HoraFin, x.Estado, x.MotivoCancelacion,
                x.FechaCreacion, x.FechaActualizacion));
        }

        [HttpPost]
        public async Task<ActionResult<CitaReadDto>> Create(CitaCreateDto dto)
        {
            var e = new Cita
            {
                IdCliente = dto.IdCliente,
                IdTecnico = dto.IdTecnico,
                IdServicio = dto.IdServicio,
                FechaCita = dto.FechaCita,
                HoraInicio = dto.HoraInicio,
                HoraFin = dto.HoraFin,
                Estado = "pendiente"
            };
            _db.Citas.Add(e);
            await _db.SaveChangesAsync();

            return CreatedAtAction(nameof(Get), new { id = e.IdCita }, new CitaReadDto(
                e.IdCita, e.IdCliente, e.IdTecnico, e.IdServicio,
                e.FechaCita, e.HoraInicio, e.HoraFin, e.Estado, e.MotivoCancelacion,
                e.FechaCreacion, e.FechaActualizacion));
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, CitaUpdateDto dto)
        {
            var e = await _db.Citas.FirstOrDefaultAsync(x => x.IdCita == id);
            if (e is null) return NotFound();

            e.IdCliente = dto.IdCliente;
            e.IdTecnico = dto.IdTecnico;
            e.IdServicio = dto.IdServicio;
            e.FechaCita = dto.FechaCita;
            e.HoraInicio = dto.HoraInicio;
            e.HoraFin = dto.HoraFin;
            e.Estado = dto.Estado;
            e.FechaActualizacion = DateTime.UtcNow;
            await _db.SaveChangesAsync();
            return NoContent();
        }

        [HttpPatch("{id:int}/cancel")]
        public async Task<IActionResult> Cancel(int id, CitaCancelDto dto)
        {
            var e = await _db.Citas.FirstOrDefaultAsync(x => x.IdCita == id);
            if (e is null) return NotFound();

            e.Estado = "cancelada";
            e.MotivoCancelacion = dto.MotivoCancelacion;
            e.FechaActualizacion = DateTime.UtcNow;
            await _db.SaveChangesAsync();
            return NoContent();
        }

        // Si prefieres "eliminación lógica" en citas, podrías marcar estado="cancelada" aquí:
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteAsCancel(int id)
        {
            var e = await _db.Citas.FirstOrDefaultAsync(x => x.IdCita == id);
            if (e is null) return NotFound();
            e.Estado = "cancelada";
            e.MotivoCancelacion ??= "Eliminada vía DELETE";
            e.FechaActualizacion = DateTime.UtcNow;
            await _db.SaveChangesAsync();
            return NoContent();
        }
    }
}