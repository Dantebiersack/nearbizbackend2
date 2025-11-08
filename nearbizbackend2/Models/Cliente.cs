using nearbizbackend.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace nearbizbackend.Models
{
    public class Cliente
    {
        [Key, Column("id_cliente")] public int IdCliente { get; set; }
        [Column("id_usuario")] public int IdUsuario { get; set; }
        public Usuario? Usuario { get; set; }
        [Column("fecha_registro")] public DateTime FechaRegistro { get; set; }
        [Column("estado")] public bool Estado { get; set; } = true;
    }
}