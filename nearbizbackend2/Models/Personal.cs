using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace nearbizbackend.Models
{
    public class Personal
    {
        [Key, Column("id_personal")] public int IdPersonal { get; set; }
        [Column("id_usuario")] public int IdUsuario { get; set; }
        public Usuario? Usuario { get; set; }
        [Column("id_negocio")] public int IdNegocio { get; set; }
        public Negocio? Negocio { get; set; }
        [Column("rol_en_negocio")] public string? RolEnNegocio { get; set; }
        [Column("fecha_registro")] public DateTime FechaRegistro { get; set; }
        [Column("estado")] public bool Estado { get; set; } = true;
    }
}