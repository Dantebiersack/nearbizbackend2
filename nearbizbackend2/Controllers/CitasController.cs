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

        // =======================
        // Helpers de validación
        // =======================
        private static bool IsQuarter(TimeOnly t) =>
            t.Minute is 0 or 15 or 30 or 45;

        private static bool IsQuarterDuration(TimeOnly start, TimeOnly end) =>
            ((end.ToTimeSpan() - start.ToTimeSpan()).TotalMinutes % 15) == 0;

        private static bool Overlaps(TimeOnly aStart, TimeOnly aEnd, TimeOnly bStart, TimeOnly bEnd) =>
            aStart < bEnd && bStart < aEnd;

        // =======================
        // Endpoints
        // =======================

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
            // 1) Validaciones de bloques de 15 y duración
            if (!IsQuarter(dto.HoraInicio) || !IsQuarter(dto.HoraFin) || !IsQuarterDuration(dto.HoraInicio, dto.HoraFin))
                return BadRequest(new { message = "Las citas deben iniciar y terminar en bloques de 15 min y su duración debe ser múltiplo de 15." });

            if (dto.HoraFin <= dto.HoraInicio)
                return BadRequest(new { message = "Hora fin debe ser mayor a hora inicio." });

            // 2) Validar solapes (mismo día, excluyendo canceladas) para cliente y técnico
            var solapaCliente = await _db.Citas
                .Where(c => c.IdCliente == dto.IdCliente && c.FechaCita == dto.FechaCita && c.Estado != "cancelada")
                .AnyAsync(c => Overlaps(c.HoraInicio, c.HoraFin, dto.HoraInicio, dto.HoraFin));

            if (solapaCliente)
                return Conflict(new { message = "El cliente ya tiene una cita en ese horario." });

            var solapaTecnico = await _db.Citas
                .Where(c => c.IdTecnico == dto.IdTecnico && c.FechaCita == dto.FechaCita && c.Estado != "cancelada")
                .AnyAsync(c => Overlaps(c.HoraInicio, c.HoraFin, dto.HoraInicio, dto.HoraFin));

            if (solapaTecnico)
                return Conflict(new { message = "El técnico ya tiene una cita en ese horario." });

            var e = new Cita
            {
                IdCliente = dto.IdCliente,
                IdTecnico = dto.IdTecnico,
                IdServicio = dto.IdServicio,
                FechaCita = dto.FechaCita,
                HoraInicio = dto.HoraInicio,
                HoraFin = dto.HoraFin,
                Estado = "pendiente",
                FechaCreacion = DateTime.UtcNow,
                FechaActualizacion = DateTime.UtcNow
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

            // 1) Validaciones de bloques de 15 y duración
            if (!IsQuarter(dto.HoraInicio) || !IsQuarter(dto.HoraFin) || !IsQuarterDuration(dto.HoraInicio, dto.HoraFin))
                return BadRequest(new { message = "Las citas deben iniciar y terminar en bloques de 15 min y su duración debe ser múltiplo de 15." });

            if (dto.HoraFin <= dto.HoraInicio)
                return BadRequest(new { message = "Hora fin debe ser mayor a hora inicio." });

            // 2) Validar solapes para cliente y técnico (excluirse a sí misma y canceladas)
            var solapaCliente = await _db.Citas
                .Where(c => c.IdCita != id && c.IdCliente == dto.IdCliente && c.FechaCita == dto.FechaCita && c.Estado != "cancelada")
                .AnyAsync(c => Overlaps(c.HoraInicio, c.HoraFin, dto.HoraInicio, dto.HoraFin));

            if (solapaCliente)
                return Conflict(new { message = "El cliente ya tiene una cita en ese horario." });

            var solapaTecnico = await _db.Citas
                .Where(c => c.IdCita != id && c.IdTecnico == dto.IdTecnico && c.FechaCita == dto.FechaCita && c.Estado != "cancelada")
                .AnyAsync(c => Overlaps(c.HoraInicio, c.HoraFin, dto.HoraInicio, dto.HoraFin));

            if (solapaTecnico)
                return Conflict(new { message = "El técnico ya tiene una cita en ese horario." });

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

        [HttpPatch("{id:int}/approve")]
        public async Task<IActionResult> Approve(int id)
        {
            var e = await _db.Citas.FirstOrDefaultAsync(x => x.IdCita == id);
            if (e is null) return NotFound();
            e.Estado = "atendida";
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

        // Si prefieres "eliminación lógica" en citas, ya cancelamos en DELETE:
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
