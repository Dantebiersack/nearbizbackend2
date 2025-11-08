using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace nearbizbackend.Models
{
    public class Promocion
    {
        [Key, Column("id_promocion")] public int IdPromocion { get; set; }
        [Column("id_negocio")] public int IdNegocio { get; set; }
        public Negocio? Negocio { get; set; }
        [Column("titulo")] public string Titulo { get; set; } = default!;
        [Column("descripcion")] public string? Descripcion { get; set; }
        [Column("fecha_inicio")] public DateOnly FechaInicio { get; set; }
        [Column("fecha_fin")] public DateOnly FechaFin { get; set; }
        [Column("estado")] public bool Estado { get; set; } = true;
    }
}