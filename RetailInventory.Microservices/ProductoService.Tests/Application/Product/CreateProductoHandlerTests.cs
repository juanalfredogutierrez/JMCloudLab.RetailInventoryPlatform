using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using ProductoService.Application.Commands.CreateProducto;
using ProductoService.Infrastructure.Persistence;
using Xunit;

namespace ProductoService.Tests.Application.Product;

public class CreateProductoHandlerTests
{
    [Fact]
    public async Task Handle_Should_Create_Product_When_Request_Is_Valid()
    {
        // Arrange

        var options = new DbContextOptionsBuilder<ProductoDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        await using var context = new ProductoDbContext(options);

        var logger = Mock.Of<ILogger<CreateProductoHandler>>();

        var handler = new CreateProductoHandler(
            context,
            logger);

        var command = new CreateProductoCommand(
            "P001",
            "Laptop",
            "Laptop Dell",
            1500);

        // Act

        var result = await handler.Handle(
            command,
            CancellationToken.None);

        // Assert

        result.IsSuccess.Should().BeTrue();

        result.Value.Should().NotBe(Guid.Empty);

        result.Errors.Should().BeEmpty();

        context.Productos.Count().Should().Be(1);

        var producto = await context.Productos.FirstAsync();

        producto.Codigo.Should().Be("P001");
        producto.Nombre.Should().Be("Laptop");
        producto.Precio.Should().Be(1500);
        producto.Activo.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_Should_Return_Failure_When_Code_Already_Exists()
    {
        // Arrange

        var options = new DbContextOptionsBuilder<ProductoDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        await using var context = new ProductoDbContext(options);

        context.Productos.Add(new ProductoService.Domain.Entities.Producto
        {
            Codigo = "P001",
            Nombre = "Producto existente",
            Descripcion = "Desc",
            Precio = 100,
            Activo = true,
            CreatedBy = "system"
        });

        await context.SaveChangesAsync();

        var logger = Mock.Of<ILogger<CreateProductoHandler>>();

        var handler = new CreateProductoHandler(
            context,
            logger);

        var command = new CreateProductoCommand(
            "P001",
            "Laptop",
            "Laptop Dell",
            1500);

        // Act

        var result = await handler.Handle(
            command,
            CancellationToken.None);

        // Assert

        result.IsFailure.Should().BeTrue();

        result.Value.Should().Be(Guid.Empty);

        result.FirstError.Should().NotBeNull();

        result.FirstError!.Code.Should().Be("Conflict");

        result.FirstError.Message.Should()
            .Be("Ya existe un producto con ese código.");

        context.Productos.Count().Should().Be(1);
    }
}