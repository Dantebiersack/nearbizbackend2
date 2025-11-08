using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace nearbizbackend.Models
{
    public class Categoria
    {
        [Key, Column("id_categoria")] public int IdCategoria { get; set; }
        [Column("nombre_categoria")] public string NombreCategoria { get; set; } = default!;
        [Column("estado")] public bool Estado { get; set; } = true;
    }
}
