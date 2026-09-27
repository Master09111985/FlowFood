using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

using FlowFood.Models;

namespace FlowFood.Data
{
  public class DataContext : IdentityDbContext
  {
    public DataContext(DbContextOptions<DataContext> options) : base(options){ }

    // Aqui van los DbSets
    public DbSet<Almacen> Almacenes { get; set; }
    public DbSet<Asistencia> Asistencias { get; set; }
    public DbSet<Baja> Bajas { get; set; }
    public DbSet<CajaTurno> CajaTurnos { get; set; }
    public DbSet<Categoria> Categorias { get; set; }
    public DbSet<Cliente> Clientes { get; set; }
    public DbSet<Comanda> Comandas { get; set; }
    public DbSet<ComandaDetalle> ComandaDetalles { get; set; }
    public DbSet<Empleado> Empleados { get; set; }
    public DbSet<Inventario> Inventarios { get; set; }
    public DbSet<MetodoPago> MetodosPago { get; set; }
    public DbSet<MotivoBaja> MotivosBaja { get; set; }
    public DbSet<MovimientoCaja> MovimientosCaja { get; set; }
    public DbSet<Permiso> Permisos { get; set; }
    public DbSet<Plataforma> Plataformas { get; set; }
    public DbSet<Platillo> Platillos { get; set; }
    public DbSet<Producto> Productos { get; set; }
    public DbSet<Proveedor> Proveedores { get; set; }
    public DbSet<Puesto> Puestos { get; set; }
    public DbSet<Rol> Roles { get; set; }
    public DbSet<RolPermiso> RolPermisos { get; set; }
    public DbSet<UnidadMedida> UnidadMedidas { get; set; }
    public DbSet<Usuario> Usuarios { get; set; }

    // Reglas de la base de datos (FLUENT API)
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      base.OnModelCreating(modelBuilder);

      // Esto es para desactivar el borrado en cascada para datos dependientes
      foreach (var relationship in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
      {
        relationship.DeleteBehavior = DeleteBehavior.Restrict;
      }

      // Evita que un Rol tenga el mismo Permiso duplicado
      modelBuilder.Entity<RolPermiso>()
          .HasIndex(rp => new { rp.rolId, rp.permisoId })
          .IsUnique();

      // Evita un codigo de empleado duplicado
      modelBuilder.Entity<Empleado>()
        .HasIndex(e => e.Codigo)
        .IsUnique();

      // Para que el indice sea unico
      modelBuilder.Entity<Inventario>()
        .HasIndex(i => new { i.productoId, i.almacenId })
        .IsUnique();
    }
  }
}
