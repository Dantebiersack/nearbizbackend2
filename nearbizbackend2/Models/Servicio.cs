using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace nearbizbackend.Models
{
    public class Servicio
    {
        [Key, Column("id_servicio")] public int IdServicio { get; set; }
        [Column("id_negocio")] public int IdNegocio { get; set; }
        public Negocio? Negocio { get; set; }
        [Column("nombre_servicio")] public string NombreServicio { get; set; } = default!;
        [Column("descripcion")] public string? Descripcion { get; set; }
        [Column("precio", TypeName = "decimal(10,2)")] public decimal? Precio { get; set; }
        [Column("duracion_minutos")] public int? DuracionMinutos { get; set; }
        [Column("estado")] public bool Estado { get; set; } = true;
    }
}
