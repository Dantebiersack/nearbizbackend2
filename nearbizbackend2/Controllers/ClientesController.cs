using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using nearbizbackend.Data;
using nearbizbackend.DTOs;
using nearbizbackend.Models;

namespace nearbizbackend2.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClientesController : ControllerBase
    {
        private readonly NearBizDbContext _db;
        public ClientesController(NearBizDbContext db) => _db = db;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ClienteReadDto>>> GetAll([FromQuery] bool includeInactive = false)
        {
            var q = includeInactive ? _db.Clientes.IgnoreQueryFilters() : _db.Clientes;
            var items = await q.AsNoTracking()
                .Select(c => new ClienteReadDto(c.IdCliente, c.IdUsuario, c.FechaRegistro, c.Estado))
                .ToListAsync();
            return Ok(items);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<ClienteReadDto>> Get(int id)
        {
            var c = await _db.Clientes.IgnoreQueryFilters().FirstOrDefaultAsync(x => x.IdCliente == id);
            if (c is null) return NotFound();
            return Ok(new ClienteReadDto(c.IdCliente, c.IdUsuario, c.FechaRegistro, c.Estado));
        }

        [HttpPost]
        public async Task<ActionResult<ClienteReadDto>> Create(ClienteCreateDto dto)
        {
            var e = new Cliente { IdUsuario = dto.IdUsuario, Estado = true };
            _db.Clientes.Add(e);
            await _db.SaveChangesAsync();
            return CreatedAtAction(nameof(Get), new { id = e.IdCliente },
                new ClienteReadDto(e.IdCliente, e.IdUsuario, e.FechaRegistro, e.Estado));
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, ClienteUpdateDto dto)
        {
            var e = await _db.Clientes.IgnoreQueryFilters().FirstOrDefaultAsync(x => x.IdCliente == id);
            if (e is null) return NotFound();
            e.IdUsuario = dto.IdUsuario;
            await _db.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> SoftDelete(int id)
        {
            var e = await _db.Clientes.IgnoreQueryFilters().FirstOrDefaultAsync(x => x.IdCliente == id);
            if (e is null) return NotFound();
            e.Estado = false;
            await _db.SaveChangesAsync();
            return NoContent();
        }

        [HttpPatch("{id:int}/restore")]
        public async Task<IActionResult> Restore(int id)
        {
            var e = await _db.Clientes.IgnoreQueryFilters().FirstOrDefaultAsync(x => x.IdCliente == id);
            if (e is null) return NotFound();
            e.Estado = true;
            await _db.SaveChangesAsync();
            return NoContent();
        }
    }
}
