using TransaccionService.Domain.Errors;

namespace TransaccionService.Application.Commands.CreateVenta;

public class CreateVentaValidator
    : AbstractValidator<CreateVentaCommand>
{
    public CreateVentaValidator()
    {
        RuleFor(x => x.Detalles)
            .NotEmpty()
            .WithErrorCode(VentaErrors.SinItems.Code)
            .WithMessage(VentaErrors.SinItems.Message);

        RuleForEach(x => x.Detalles)
            .ChildRules(detalle =>
            {
                detalle.RuleFor(x => x.ProductoId)
                    .GreaterThan(0)
                    .WithErrorCode(VentaErrors.ProductoInvalido.Code)
                    .WithMessage(VentaErrors.ProductoInvalido.Message);

                detalle.RuleFor(x => x.Cantidad)
                    .GreaterThan(0)
                    .WithErrorCode(VentaErrors.CantidadInvalida.Code)
                    .WithMessage(VentaErrors.CantidadInvalida.Message);

                detalle.RuleFor(x => x.PrecioUnitario)
                    .GreaterThan(0)
                    .WithErrorCode(VentaErrors.PrecioInvalido.Code)
                    .WithMessage(VentaErrors.PrecioInvalido.Message);
            });
    }
}