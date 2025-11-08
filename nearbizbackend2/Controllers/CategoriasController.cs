using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using nearbizbackend.Data;
using nearbizbackend.DTOs;
using nearbizbackend.Models;

namespace nearbizbackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoriasController : ControllerBase
    {
        private readonly NearBizDbContext _db;
        public CategoriasController(NearBizDbContext db) => _db = db;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CategoriaReadDto>>> GetAll([FromQuery] bool includeInactive = false)
        {
            var q = includeInactive ? _db.Categorias.IgnoreQueryFilters() : _db.Categorias;
            var items = await q.AsNoTracking()
                .Select(x => new CategoriaReadDto(x.IdCategoria, x.NombreCategoria, x.Estado))
                .ToListAsync();
            return Ok(items);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<CategoriaReadDto>> Get(int id)
        {
            var x = await _db.Categorias.IgnoreQueryFilters().FirstOrDefaultAsync(r => r.IdCategoria == id);
            if (x is null) return NotFound();
            return Ok(new CategoriaReadDto(x.IdCategoria, x.NombreCategoria, x.Estado));
        }

        [HttpPost]
        public async Task<ActionResult<CategoriaReadDto>> Create(CategoriaCreateDto dto)
        {
            var e = new Categoria { NombreCategoria = dto.NombreCategoria, Estado = true };
            _db.Categorias.Add(e);
            await _db.SaveChangesAsync();
            return CreatedAtAction(nameof(Get), new { id = e.IdCategoria }, new CategoriaReadDto(e.IdCategoria, e.NombreCategoria, e.Estado));
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, CategoriaUpdateDto dto)
        {
            var e = await _db.Categorias.IgnoreQueryFilters().FirstOrDefaultAsync(x => x.IdCategoria == id);
            if (e is null) return NotFound();
            e.NombreCategoria = dto.NombreCategoria;
            await _db.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> SoftDelete(int id)
        {
            var e = await _db.Categorias.IgnoreQueryFilters().FirstOrDefaultAsync(x => x.IdCategoria == id);
            if (e is null) return NotFound();
            e.Estado = false;
            await _db.SaveChangesAsync();
            return NoContent();
        }

        [HttpPatch("{id:int}/restore")]
        public async Task<IActionResult> Restore(int id)
        {
            var e = await _db.Categorias.IgnoreQueryFilters().FirstOrDefaultAsync(x => x.IdCategoria == id);
            if (e is null) return NotFound();
            e.Estado = true;
            await _db.SaveChangesAsync();
            return NoContent();
        }
    }
}
