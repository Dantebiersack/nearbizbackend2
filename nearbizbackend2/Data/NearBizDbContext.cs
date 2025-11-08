using Microsoft.EntityFrameworkCore;
using nearbizbackend.Models;

namespace nearbizbackend.Data
{
    public class NearBizDbContext : DbContext
    {
        public NearBizDbContext(DbContextOptions<NearBizDbContext> options) : base(options) { }

        public DbSet<Rol> Roles => Set<Rol>();
        public DbSet<Usuario> Usuarios => Set<Usuario>();
        public DbSet<Categoria> Categorias => Set<Categoria>();
        public DbSet<Negocio> Negocios => Set<Negocio>();
        public DbSet<Membresia> Membresias => Set<Membresia>();
        public DbSet<Cliente> Clientes => Set<Cliente>();
        public DbSet<Personal> Personal => Set<Personal>();
        public DbSet<Servicio> Servicios => Set<Servicio>();
        public DbSet<Cita> Citas => Set<Cita>();
        public DbSet<Valoracion> Valoraciones => Set<Valoracion>();
        public DbSet<Promocion> Promociones => Set<Promocion>();

        protected override void OnModelCreating(ModelBuilder b)
        {
            // Global filters de eliminación lógica (solo tablas con BIT estado)
            b.Entity<Rol>().HasQueryFilter(x => x.Estado);
            b.Entity<Usuario>().HasQueryFilter(x => x.Estado);
            b.Entity<Categoria>().HasQueryFilter(x => x.Estado);
            b.Entity<Negocio>().HasQueryFilter(x => x.Estado);
            b.Entity<Membresia>().HasQueryFilter(x => x.Estado);
            b.Entity<Cliente>().HasQueryFilter(x => x.Estado);
            b.Entity<Personal>().HasQueryFilter(x => x.Estado);
            b.Entity<Servicio>().HasQueryFilter(x => x.Estado);
            b.Entity<Valoracion>().HasQueryFilter(x => x.Estado);
            b.Entity<Promocion>().HasQueryFilter(x => x.Estado);

            b.Entity<Usuario>(e =>
            {
                e.ToTable("Usuarios");
                e.HasKey(x => x.IdUsuario);
                e.HasIndex(x => x.Email).IsUnique();
                e.Property(x => x.Nombre).HasMaxLength(100).IsRequired();
                e.Property(x => x.Email).HasMaxLength(100).IsRequired();

                // Si puedes, evita caracteres no ASCII en nombres físicos:
                // e.Property(x => x.ContrasenaHash).HasColumnName("contrasena_hash").HasMaxLength(255).IsRequired();
                e.Property(x => x.ContrasenaHash).HasColumnName("contrasena_hash").HasMaxLength(255).IsRequired();

                // GETDATE() -> now()
                e.Property(x => x.FechaRegistro).HasDefaultValueSql("now()");
                e.Property(x => x.Estado).HasDefaultValue(true);
                e.Property(x => x.Token).HasColumnName("token");
                e.HasOne(x => x.Rol).WithMany().HasForeignKey(x => x.IdRol).OnDelete(DeleteBehavior.Restrict);
            });

            b.Entity<Cliente>(e =>
            {
                e.ToTable("Clientes");
                e.HasKey(x => x.IdCliente);
                e.Property(x => x.FechaRegistro).HasDefaultValueSql("now()");
                e.Property(x => x.Estado).HasDefaultValue(true);
                e.HasOne(x => x.Usuario).WithMany().HasForeignKey(x => x.IdUsuario).OnDelete(DeleteBehavior.Restrict);
            });

            b.Entity<Personal>(e =>
            {
                e.ToTable("Personal");
                e.HasKey(x => x.IdPersonal);
                e.Property(x => x.RolEnNegocio).HasMaxLength(50);
                e.Property(x => x.FechaRegistro).HasDefaultValueSql("now()");
                e.Property(x => x.Estado).HasDefaultValue(true);
                e.HasOne(x => x.Usuario).WithMany().HasForeignKey(x => x.IdUsuario).OnDelete(DeleteBehavior.Restrict);
                e.HasOne(x => x.Negocio).WithMany().HasForeignKey(x => x.IdNegocio).OnDelete(DeleteBehavior.Cascade);
            });

            b.Entity<Cita>(e =>
            {
                e.ToTable("Citas");
                e.HasKey(x => x.IdCita);
                e.Property(x => x.Estado).HasMaxLength(30).HasDefaultValue("pendiente");
                e.Property(x => x.MotivoCancelacion).HasMaxLength(255);

                // GETDATE() -> now()
                e.Property(x => x.FechaCreacion).HasDefaultValueSql("now()");
                e.Property(x => x.FechaActualizacion).HasDefaultValueSql("now()");

                // Si quieres timezone real en columnas:
                // e.Property(x => x.FechaCreacion).HasColumnType("timestamp with time zone").HasDefaultValueSql("now()");
                // e.Property(x => x.FechaActualizacion).HasColumnType("timestamp with time zone").HasDefaultValueSql("now()");

                e.HasOne(x => x.Cliente).WithMany().HasForeignKey(x => x.IdCliente).OnDelete(DeleteBehavior.Restrict);
                e.HasOne(x => x.Tecnico).WithMany().HasForeignKey(x => x.IdTecnico).OnDelete(DeleteBehavior.Restrict);
                e.HasOne(x => x.Servicio).WithMany().HasForeignKey(x => x.IdServicio).OnDelete(DeleteBehavior.Restrict);

                // Tu filtro por navegación está bien; solo ojo con nulos.
                e.HasQueryFilter(c => c.Cliente.Estado && c.Tecnico.Estado && c.Servicio.Estado);
            });

            b.Entity<Negocio>(e =>
            {
                e.ToTable("Negocios");
                e.HasKey(x => x.IdNegocio);
                e.Property(x => x.Nombre).HasMaxLength(100).IsRequired();
                e.Property(x => x.Direccion).HasMaxLength(255);
                e.Property(x => x.LinkUrl).HasColumnName("linkUrl");
                e.Property(x => x.Estado).HasDefaultValue(true);
                e.HasOne(x => x.Categoria).WithMany().HasForeignKey(x => x.IdCategoria).OnDelete(DeleteBehavior.Restrict);

                // Precisión sigue bien en Postgres
                e.Property(x => x.CoordenadasLat).HasPrecision(10, 7);
                e.Property(x => x.CoordenadasLng).HasPrecision(10, 7);
            });

            b.Entity<Membresia>(e =>
            {
                e.ToTable("Membresias");
                e.HasKey(x => x.IdMembresia);
                e.Property(x => x.PrecioMensual).HasColumnType("numeric(10,2)"); // opcional: antes tenías "decimal(10,2)"
                e.Property(x => x.Estado).HasDefaultValue(true);
                e.HasOne(x => x.Negocio).WithMany(x => x.Membresias).HasForeignKey(x => x.IdNegocio).OnDelete(DeleteBehavior.Cascade);
            });

            b.Entity<Servicio>(e =>
            {
                e.ToTable("Servicios");
                e.HasKey(x => x.IdServicio);
                e.Property(x => x.NombreServicio).HasMaxLength(100).IsRequired();
                e.Property(x => x.Precio).HasColumnType("numeric(10,2)"); // opcional
                e.Property(x => x.Estado).HasDefaultValue(true);
                e.HasOne(x => x.Negocio).WithMany().HasForeignKey(x => x.IdNegocio).OnDelete(DeleteBehavior.Cascade);
            });

            b.Entity<Valoracion>(e =>
            {
                e.ToTable("Valoraciones");
                e.HasKey(x => x.IdValoracion);
                e.Property(x => x.Calificacion);
                e.Property(x => x.Fecha).HasDefaultValueSql("now()");
                e.Property(x => x.Estado).HasDefaultValue(true);
                e.HasOne(x => x.Cita).WithMany().HasForeignKey(x => x.IdCita).OnDelete(DeleteBehavior.Cascade);
            });

            b.Entity<Promocion>(e =>
            {
                e.ToTable("Promociones");
                e.HasKey(x => x.IdPromocion);
                e.Property(x => x.Titulo).HasMaxLength(150).IsRequired();
                e.Property(x => x.Estado).HasDefaultValue(true);
                e.HasOne(x => x.Negocio).WithMany().HasForeignKey(x => x.IdNegocio).OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
