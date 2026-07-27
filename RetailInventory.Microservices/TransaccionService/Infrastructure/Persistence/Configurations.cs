using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TransaccionService.Domain.Entities;

namespace TransaccionService.Infrastructure.Persistence.Configurations;

public sealed class OutboxMessageConfiguration
    : IEntityTypeConfiguration<OutboxMessage>
{
    public void Configure(EntityTypeBuilder<OutboxMessage> builder)
    {
        builder.ToTable("OutboxMessage");

        builder.HasKey(x => x.Id);

        builder.HasIndex(x => x.Uid)
            .IsUnique();

        builder.Property(x => x.EventType)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.Payload)
            .HasColumnType("nvarchar(max)")
            .IsRequired();

        builder.Property(x => x.Error)
            .HasColumnType("nvarchar(max)");

        builder.Property(x => x.OccurredOn)
            .IsRequired();
    }
}