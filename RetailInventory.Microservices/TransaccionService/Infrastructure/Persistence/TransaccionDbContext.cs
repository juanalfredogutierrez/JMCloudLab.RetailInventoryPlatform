using BuildingBlocks.Domain;
using TransaccionService.Domain.Entities;

namespace TransaccionService.Infrastructure.Persistence;

public class TransaccionDbContext : BaseDbContext
{
    public TransaccionDbContext(DbContextOptions<TransaccionDbContext> options)
        : base(options)
    {
    }

    public DbSet<Compra> Compras => Set<Compra>();
    public DbSet<DetalleCompra> DetalleCompras => Set<DetalleCompra>();
    public DbSet<Venta> Ventas => Set<Venta>();
    public DbSet<DetalleVenta> DetalleVentas => Set<DetalleVenta>();
    public DbSet<OutboxMessage> OutboxMessages =>Set<OutboxMessage>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // COMPRA
        modelBuilder.Entity<Compra>(entity =>
        {
            entity.ToTable("Compra");

            entity.HasKey(x => x.Id);

            entity.Property(x => x.NumeroCompra)
                  .IsRequired()
                  .HasMaxLength(50);

            entity.Property(x => x.TotalCompra)
                  .HasColumnType("decimal(18,2)");

            entity.HasMany(x => x.Detalles)
                  .WithOne()
                  .HasForeignKey("CompraId");
        });

        // VENTA
        modelBuilder.Entity<Venta>(entity =>
        {
            entity.ToTable("Venta");

            entity.HasKey(x => x.Id);

            entity.Property(x => x.NumeroVenta)
                  .IsRequired()
                  .HasMaxLength(50);

            entity.Property(x => x.TotalVenta)
                  .HasColumnType("decimal(18,2)");

            entity.HasMany(x => x.Detalles)
                  .WithOne()
                  .HasForeignKey("VentaId");
        });

        // DETALLE COMPRA
        modelBuilder.Entity<DetalleCompra>(entity =>
        {
            entity.ToTable("DetalleCompra");

            entity.HasKey(x => x.Id);

            entity.Property(x => x.PrecioUnitario)
                  .HasColumnType("decimal(18,2)");

            entity.Property(x => x.Subtotal)
                  .HasColumnType("decimal(18,2)");
        });

        // DETALLE VENTA
        modelBuilder.Entity<DetalleVenta>(entity =>
        {
            entity.ToTable("DetalleVenta");

            entity.HasKey(x => x.Id);

            entity.Property(x => x.PrecioUnitario)
                  .HasColumnType("decimal(18,2)");

            entity.Property(x => x.Subtotal)
                  .HasColumnType("decimal(18,2)");
        });

        // OUTBOX
        modelBuilder.Entity<OutboxMessage>(entity =>
        {
            entity.ToTable("OutboxMessage");

            entity.HasKey(x => x.Id);

            entity.HasIndex(x => x.Uid)
                  .IsUnique();

            entity.Property(x => x.EventType)
                  .IsRequired()
                  .HasMaxLength(200);

            entity.Property(x => x.Payload)
                  .IsRequired();

            entity.Property(x => x.OccurredOn)
                  .IsRequired();

            entity.Property(x => x.Error);
        });
    }

}