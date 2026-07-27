using FluentValidation;

namespace InventarioService.Application.Commands.RegistrarSalida;

public class RegistrarSalidaValidator
    : AbstractValidator<RegistrarSalidaCommand>
{
    public RegistrarSalidaValidator()
    {
        RuleFor(x => x.ProductoId)
            .GreaterThan(0)
            .WithMessage("El producto es obligatorio.");

        RuleFor(x => x.Cantidad)
            .GreaterThan(0)
            .WithMessage("La cantidad debe ser mayor a cero.");
    }
}