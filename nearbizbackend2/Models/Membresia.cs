using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace nearbizbackend.Models
{
    public class Membresia
    {
        [Key, Column("id_membresia")] public int IdMembresia { get; set; }

        [Column("precio_mensual", TypeName = "decimal(10,2)")]
        public decimal? PrecioMensual { get; set; }

        [Column("id_negocio")]
        public int IdNegocio { get; set; }
        public Negocio? Negocio { get; set; }

        [Column("estado")]
        public bool Estado { get; set; } = true;

        [Column("ultima_renovacion", TypeName = "timestamp with time zone")]
        public DateTime? UltimaRenovacion { get; set; }
    }
}
