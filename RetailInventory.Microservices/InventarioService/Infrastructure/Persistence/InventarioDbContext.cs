using BuildingBlocks.Domain;
using InventarioService.Domain.Entities;

namespace InventarioService.Infrastructure.Persistence;

public class InventarioDbContext : BaseDbContext
{
    public InventarioDbContext(DbContextOptions<InventarioDbContext> options)
        : base(options) { }

    public DbSet<MovimientoInventario> Movimientos => Set<MovimientoInventario>();
    public DbSet<DetalleMovimientoInventario> Detalles => Set<DetalleMovimientoInventario>();
    public DbSet<ExistenciaProducto> Existencias => Set<ExistenciaProducto>();
    public DbSet<EventoProcesado> EventosProcesados => Set<EventoProcesado>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<MovimientoInventario>().ToTable("MovimientoInventario");
        modelBuilder.Entity<DetalleMovimientoInventario>().ToTable("DetalleMovimientoInventario");
        modelBuilder.Entity<ExistenciaProducto>().ToTable("ExistenciaProducto");
        modelBuilder.Entity<EventoProcesado>().ToTable("EventoProcesado");
    }

}