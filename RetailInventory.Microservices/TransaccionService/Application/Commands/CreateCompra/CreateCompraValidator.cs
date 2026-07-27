using TransaccionService.Domain.Errors;

namespace TransaccionService.Application.Commands.CreateCompra
{
    public class CreateCompraValidator
    : AbstractValidator<CreateCompraCommand>
    {
        public CreateCompraValidator()
        {
            RuleFor(x => x.Detalles)
            .NotEmpty()
            .WithErrorCode(CompraErrors.SinItems.Code)
            .WithMessage(CompraErrors.SinItems.Message);

            RuleForEach(x => x.Detalles)
            .ChildRules(detalle =>
            {
                detalle.RuleFor(x => x.ProductoId)
                .GreaterThan(0)
                .WithErrorCode(CompraErrors.ProductoInvalido.Code)
                .WithMessage(CompraErrors.ProductoInvalido.Message);

                 detalle.RuleFor(x => x.Cantidad)
                .GreaterThan(0)
                .WithErrorCode(CompraErrors.CantidadInvalida.Code)
                .WithMessage(CompraErrors.CantidadInvalida.Message);

                detalle.RuleFor(x => x.PrecioUnitario)
                .GreaterThan(0)
                .WithErrorCode(CompraErrors.PrecioInvalido.Code)
                .WithMessage(CompraErrors.PrecioInvalido.Message);
            });
        }
    }
}
