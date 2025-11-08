using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace nearbizbackend.Models
{
    public class Cita
    {
        [Key, Column("id_cita")] public int IdCita { get; set; }
        [Column("id_cliente")] public int IdCliente { get; set; }
        public Cliente? Cliente { get; set; }
        [Column("id_tecnico")] public int IdTecnico { get; set; } // Personal
        public Personal? Tecnico { get; set; }
        [Column("id_servicio")] public int IdServicio { get; set; }
        public Servicio? Servicio { get; set; }
        [Column("fecha_cita")] public DateOnly FechaCita { get; set; }
        [Column("hora_inicio")] public TimeOnly HoraInicio { get; set; }
        [Column("hora_fin")] public TimeOnly HoraFin { get; set; }
        [Column("estado")] public string Estado { get; set; } = "pendiente";
        [Column("motivo_cancelacion")] public string? MotivoCancelacion { get; set; }
        [Column("fecha_creacion")] public DateTime FechaCreacion { get; set; }
        [Column("fecha_actualizacion")] public DateTime FechaActualizacion { get; set; }
    }
}