using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace nearbizbackend.Models
{
    public class Valoracion
    {
        [Key, Column("id_valoracion")] public int IdValoracion { get; set; }
        [Column("id_cita")] public int IdCita { get; set; }
        public Cita? Cita { get; set; }

        [Column("id_cliente")] public int IdCliente { get; set; }  // sin FK explícita
        [Column("id_negocio")] public int IdNegocio { get; set; }  // sin FK explícita

        [Column("calificacion")] public int Calificacion { get; set; }
        [Column("comentario")] public string? Comentario { get; set; }
        [Column("fecha")] public DateTime Fecha { get; set; }
        [Column("estado")] public bool Estado { get; set; } = true;
    }
}
