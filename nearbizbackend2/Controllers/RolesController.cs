using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using nearbizbackend.Data;
using nearbizbackend.DTOs;
using nearbizbackend.Models;

namespace nearbizbackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RolesController : ControllerBase
    {
        private readonly NearBizDbContext _db;
        public RolesController(NearBizDbContext db) => _db = db;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<RolReadDto>>> GetAll([FromQuery] bool includeInactive = false)
        {
            var q = includeInactive ? _db.Roles.IgnoreQueryFilters() : _db.Roles;
            var items = await q.AsNoTracking()
                .Select(x => new RolReadDto(x.IdRol, x.RolNombre, x.Estado))
                .ToListAsync();
            return Ok(items);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<RolReadDto>> Get(int id)
        {
            var x = await _db.Roles.IgnoreQueryFilters().FirstOrDefaultAsync(r => r.IdRol == id);
            if (x is null) return NotFound();
            return Ok(new RolReadDto(x.IdRol, x.RolNombre, x.Estado));
        }

        [HttpPost]
        public async Task<ActionResult<RolReadDto>> Create(RolCreateDto dto)
        {
            var e = new Rol { RolNombre = dto.RolNombre, Estado = true };
            _db.Roles.Add(e);
            await _db.SaveChangesAsync();
            return CreatedAtAction(nameof(Get), new { id = e.IdRol }, new RolReadDto(e.IdRol, e.RolNombre, e.Estado));
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, RolUpdateDto dto)
        {
            var e = await _db.Roles.IgnoreQueryFilters().FirstOrDefaultAsync(x => x.IdRol == id);
            if (e is null) return NotFound();
            e.RolNombre = dto.RolNombre;
            await _db.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> SoftDelete(int id)
        {
            var e = await _db.Roles.IgnoreQueryFilters().FirstOrDefaultAsync(x => x.IdRol == id);
            if (e is null) return NotFound();
            e.Estado = false;
            await _db.SaveChangesAsync();
            return NoContent();
        }

        [HttpPatch("{id:int}/restore")]
        public async Task<IActionResult> Restore(int id)
        {
            var e = await _db.Roles.IgnoreQueryFilters().FirstOrDefaultAsync(x => x.IdRol == id);
            if (e is null) return NotFound();
            e.Estado = true;
            await _db.SaveChangesAsync();
            return NoContent();
        }
    }
}
