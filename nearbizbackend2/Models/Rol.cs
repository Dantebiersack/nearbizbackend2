using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace nearbizbackend.Models
{
    public class Rol
    {
        [Key, Column("id_rol")] public int IdRol { get; set; }
        [Column("rol")] public string RolNombre { get; set; } = default!;
        [Column("estado")] public bool Estado { get; set; } = true;
    }
}