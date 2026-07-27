using BuildingBlocks.Domain;
using Microsoft.EntityFrameworkCore;
using ProductoService.Domain.Entities;

namespace ProductoService.Infrastructure.Persistence;

public class ProductoDbContext : BaseDbContext  
{
    public ProductoDbContext(DbContextOptions<ProductoDbContext> options)
        : base(options)
    {
    }

    public DbSet<Producto> Productos => Set<Producto>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Producto>().ToTable("Producto");

        modelBuilder.Entity<Producto>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity.Property(x => x.Codigo).IsRequired().HasMaxLength(50);
            entity.Property(x => x.Nombre).IsRequired().HasMaxLength(150);
            entity.Property(x => x.Precio).HasColumnType("decimal(18,2)");
        });
    }
  
}