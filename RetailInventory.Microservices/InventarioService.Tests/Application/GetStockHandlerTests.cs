using FluentAssertions;
using InventarioService.Application.Queries.GetStock;
using InventarioService.Domain.Entities;
using InventarioService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace InventarioService.Tests.Application.Inventario;

public class GetStockHandlerTests
{
    [Fact]
    public async Task Handle_Should_Return_Stock_When_Product_Exists()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<InventarioDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        await using var context = new InventarioDbContext(options);

        context.Existencias.Add(new ExistenciaProducto
        {
            ProductoId = 1,
            CantidadDisponible = 10,
            FechaActualizacion = DateTime.Now,
            CreatedBy = "system"
        });

        await context.SaveChangesAsync();

        var handler = new GetStockHandler(context);
        var query = new GetStockQuery(ProductoId: 1);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(10);
    }

    [Fact]
    public async Task Handle_Should_Return_Zero_When_Product_Does_Not_Exist()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<InventarioDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        await using var context = new InventarioDbContext(options);

        var handler = new GetStockHandler(context);
        var query = new GetStockQuery(ProductoId: 1);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(0);
    }

    [Fact]
    public async Task Handle_Should_Return_Stock_Only_For_Requested_Product()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<InventarioDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        await using var context = new InventarioDbContext(options);

        context.Existencias.AddRange(
            new ExistenciaProducto
            {
                ProductoId = 1,
                CantidadDisponible = 10,
                FechaActualizacion = DateTime.Now,
                CreatedBy = "system"
            },
            new ExistenciaProducto
            {
                ProductoId = 2,
                CantidadDisponible = 25,
                FechaActualizacion = DateTime.Now,
                CreatedBy = "system"
            });

        await context.SaveChangesAsync();

        var handler = new GetStockHandler(context);
        var query = new GetStockQuery(ProductoId: 2);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(25);
    }
}
