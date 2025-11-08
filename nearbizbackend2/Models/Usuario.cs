using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace nearbizbackend.Models
{
    public class Usuario
    {
        [Key, Column("id_usuario")] public int IdUsuario { get; set; }
        [Column("nombre")] public string Nombre { get; set; } = default!;
        [Column("email")] public string Email { get; set; } = default!;
        [Column("contraseña_hash")] public string ContrasenaHash { get; set; } = default!;
        [Column("id_rol")] public int IdRol { get; set; }
        public Rol? Rol { get; set; }
        [Column("fecha_registro")] public DateTime FechaRegistro { get; set; }
        [Column("estado")] public bool Estado { get; set; } = true;
        [Column("token")] public string? Token { get; set; }
    }
}