using FluentAssertions;
using TransaccionService.Application.Commands.CreateCompra;
using Xunit;

namespace TransaccionService.Tests.Application;

public class CreateCompraValidatorTests
{
    private readonly CreateCompraValidator _validator;

    public CreateCompraValidatorTests()
    {
        _validator = new CreateCompraValidator();
    }

    [Fact]
    public void Should_Have_Error_When_Detalles_Are_Empty()
    {
        // Arrange
        var command = new CreateCompraCommand(
            new List<DetalleCompraDto>(),
            "Observación");

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();

        result.Errors.Should()
            .Contain(x =>
                x.PropertyName == nameof(CreateCompraCommand.Detalles));
    }

    [Fact]
    public void Should_Have_Error_When_Cantidad_Is_Invalid()
    {
        // Arrange
        var command = new CreateCompraCommand(
            new()
            {
                new DetalleCompraDto(1, 0, 100)
            },
            "Observación");

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();

        result.Errors.Should()
            .Contain(x =>
                x.PropertyName.Contains(nameof(DetalleCompraDto.Cantidad)));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-10)]
    public void Should_Have_Error_When_Precio_Is_Invalid(decimal precio)
    {
        // Arrange
        var command = new CreateCompraCommand(
            new()
            {
                new DetalleCompraDto(1, 2, precio)
            },
            "Observación");

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();

        result.Errors.Should()
            .Contain(x =>
                x.PropertyName.Contains(nameof(DetalleCompraDto.PrecioUnitario)));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Should_Have_Error_When_Producto_Is_Invalid(int productoId)
    {
        // Arrange
        var command = new CreateCompraCommand(
            new()
            {
                new DetalleCompraDto(productoId, 2, 100)
            },
            "Observación");

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();

        result.Errors.Should()
            .Contain(x =>
                x.PropertyName.Contains(nameof(DetalleCompraDto.ProductoId)));
    }

    [Fact]
    public void Should_Not_Have_Error_When_Command_Is_Valid()
    {
        // Arrange
        var command = new CreateCompraCommand(
            new()
            {
                new DetalleCompraDto(1, 2, 100),
                new DetalleCompraDto(2, 1, 50)
            },
            "Compra válida");

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeTrue();

        result.Errors.Should().BeEmpty();
    }
    [Fact]
    public void Should_Have_Multiple_Errors_When_Command_Is_Invalid()
    {
        var command = new CreateCompraCommand(
            new()
            {
            new DetalleCompraDto(0, 0, 0)
            },
            string.Empty);

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();

        result.Errors.Should().HaveCountGreaterThan(1);
    }
    [Fact]
    public void Should_Return_Multiple_Errors_When_Command_Is_Invalid()
    {
        var command = new CreateCompraCommand(
            new()
            {
             new DetalleCompraDto(0, 0, 0)
            },
            string.Empty);

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();

        result.Errors.Count.Should().BeGreaterThan(1);
    }
}