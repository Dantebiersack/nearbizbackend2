using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace nearbizbackend.Models
{
    public class Negocio
    {
        [Key, Column("id_negocio")] public int IdNegocio { get; set; }
        [Column("id_categoria")] public int IdCategoria { get; set; }
        public Categoria? Categoria { get; set; }

        [Column("id_membresia")] public int? IdMembresia { get; set; } // sin FK para evitar circularidad

        [Column("nombre")] public string Nombre { get; set; } = default!;
        [Column("direccion")] public string? Direccion { get; set; }
        [Column("coordenadas_lat")] public decimal? CoordenadasLat { get; set; }
        [Column("coordenadas_lng")] public decimal? CoordenadasLng { get; set; }
        [Column("descripcion")] public string? Descripcion { get; set; }
        [Column("telefono_contacto")] public string? TelefonoContacto { get; set; }
        [Column("correo_contacto")] public string? CorreoContacto { get; set; }
        [Column("horario_atencion")] public string? HorarioAtencion { get; set; }
        [Column("estado")] public bool Estado { get; set; } = true;
        [Column("linkUrl")] public string? LinkUrl { get; set; }

        public ICollection<Membresia> Membresias { get; set; } = new List<Membresia>();
    }
}