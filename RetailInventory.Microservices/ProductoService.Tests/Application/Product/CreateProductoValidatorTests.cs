using ProductoService.Application.Commands.CreateProducto;
using Xunit;

namespace ProductoService.Tests.Application.Product;

public class CreateProductoValidatorTests
{
    private readonly CreateProductoValidator _validator = new();

    [Fact]
    public void Should_Have_Error_When_Codigo_Is_Empty()
    {
        var command = new CreateProductoCommand(
            "",
            "Laptop",
            "Laptop Dell",
            1500);

        var result = _validator.Validate(command);

        Assert.False(result.IsValid);

        Assert.Contains(
            result.Errors,
            e => e.PropertyName == nameof(command.Codigo));
    }

    [Fact]
    public void Should_Have_Error_When_Nombre_Is_Empty()
    {
        var command = new CreateProductoCommand(
            "P001",
            "",
            "Laptop Dell",
            1500);

        var result = _validator.Validate(command);

        Assert.False(result.IsValid);

        Assert.Contains(
            result.Errors,
            e => e.PropertyName == nameof(command.Nombre));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Should_Have_Error_When_Precio_Is_Invalid(decimal precio)
    {
        var command = new CreateProductoCommand(
            "P001",
            "Laptop",
            "Laptop Dell",
            precio);

        var result = _validator.Validate(command);

        Assert.False(result.IsValid);

        Assert.Contains(
            result.Errors,
            e => e.PropertyName == nameof(command.Precio));
    }
}