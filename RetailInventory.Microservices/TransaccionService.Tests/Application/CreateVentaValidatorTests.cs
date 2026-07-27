using TransaccionService.Application.Commands.CreateVenta;
using TransaccionService.Domain.Errors;

namespace TransaccionService.Tests.Application
{
   public class CreateVentaValidatorTests
    {
        private readonly CreateVentaValidator _validator;

        public CreateVentaValidatorTests()
        {
            _validator = new CreateVentaValidator();
        }

        [Fact]
        public async Task Handle_Should_Return_Failure_When_No_Items()
        {
            var command = new CreateVentaCommand(
            new List<DetalleVentaDto>(),
            "Venta prueba");

            var result = await _validator.ValidateAsync(command);

            Assert.False(result.IsValid);
            Assert.Equal(
            VentaErrors.SinItems.Code,
            result.Errors.FirstOrDefault()?.ErrorCode);
        }

        [Fact]
        public async Task Should_Return_Failure_When_Producto_Is_Invalid()
        {
            var command = new CreateVentaCommand(
            new()
            {
              new DetalleVentaDto(0, 1, 100)
            }, "Venta");

            var result = await _validator.ValidateAsync(command );

            Assert.False(result.IsValid);

            Assert.Equal(
            VentaErrors.ProductoInvalido.Code,
            result.Errors.FirstOrDefault()?.ErrorCode);
        }

        [Fact]
        public async Task Should_Return_Failure_When_Cantidad_Is_Invalid()
        {
            var command = new CreateVentaCommand(
            new()
            {
                new DetalleVentaDto(1, 0, 100)
            },
            "Venta");

            var result = await _validator.ValidateAsync(command);

            Assert.False(result.IsValid);

            Assert.Equal(
            VentaErrors.CantidadInvalida.Code,
            result.Errors.FirstOrDefault()?.ErrorCode);
        }

        [Fact]
        public async Task Should_Return_Failure_When_Precio_Is_Invalid()
        {
            var command = new CreateVentaCommand(
            new()
            {
    new DetalleVentaDto(1, 2, 0)
            },
            "Venta");

            var result = await _validator.ValidateAsync(command);

            Assert.False(result.IsValid);
            Assert.Equal(
            VentaErrors.PrecioInvalido.Code,
            result.Errors.FirstOrDefault()?.ErrorCode);
        }

    }

}