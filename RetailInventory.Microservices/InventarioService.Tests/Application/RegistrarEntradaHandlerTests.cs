using FluentAssertions;
using InventarioService.Application.Commands.RegistrarEntrada;
using InventarioService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;

namespace InventarioService.Tests.Application.Inventario;

public class RegistrarEntradaHandlerTests
{
    [Fact]
    public async Task Handle_Should_Create_Stock_When_Product_Does_Not_Exist()
    {
        // Arrange

        var options = new DbContextOptionsBuilder<InventarioDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        await using var context = new InventarioDbContext(options);

        var logger = Mock.Of<ILogger<RegistrarEntradaHandler>>();

        var handler = new RegistrarEntradaHandler(
            context,
            logger);

        var command = new RegistrarEntradaCommand(
            ProductoId: 1,
            Cantidad: 10,
            Referencia: "OC-001");

        // Act

        var result = await handler.Handle(
            command,
            CancellationToken.None);

        // Assert

        result.Should().BeTrue();

        context.Existencias.Count().Should().Be(1);

        var stock = await context.Existencias.FirstAsync();

        stock.ProductoId.Should().Be(1);

        stock.CantidadDisponible.Should().Be(10);
    }

    [Fact]
    public async Task Handle_Should_Increase_Stock_When_Product_Already_Exists()
    {
        // Arrange

        var options = new DbContextOptionsBuilder<InventarioDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        await using var context = new InventarioDbContext(options);

        context.Existencias.Add(new InventarioService.Domain.Entities.ExistenciaProducto
        {
            ProductoId = 1,
            CantidadDisponible = 10,
            FechaActualizacion = DateTime.Now,
            CreatedBy = "system"
        });

        await context.SaveChangesAsync();

        var logger = Mock.Of<ILogger<RegistrarEntradaHandler>>();

        var handler = new RegistrarEntradaHandler(
            context,
            logger);

        var command = new RegistrarEntradaCommand(
            ProductoId: 1,
            Cantidad: 5,
            Referencia: "OC-002");

        // Act

        var result = await handler.Handle(
            command,
            CancellationToken.None);

        // Assert

        result.Should().BeTrue();

        context.Existencias.Count().Should().Be(1);

        var stock = await context.Existencias.FirstAsync();

        stock.CantidadDisponible.Should().Be(15);
    }
}