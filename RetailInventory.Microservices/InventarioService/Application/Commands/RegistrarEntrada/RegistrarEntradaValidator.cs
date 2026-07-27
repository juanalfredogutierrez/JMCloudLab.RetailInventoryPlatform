using FluentValidation;

namespace InventarioService.Application.Commands.RegistrarEntrada;

public class RegistrarEntradaValidator
    : AbstractValidator<RegistrarEntradaCommand>
{
    public RegistrarEntradaValidator()
    {
        RuleFor(x => x.ProductoId)
            .GreaterThan(0)
            .WithMessage("El producto es obligatorio.");

        RuleFor(x => x.Cantidad)
            .GreaterThan(0)
            .WithMessage("La cantidad debe ser mayor que cero.");

        RuleFor(x => x.Referencia)
            .NotEmpty()
            .WithMessage("La referencia es obligatoria.")
            .MaximumLength(50)
            .WithMessage("La referencia no puede superar los 50 caracteres.");
    }
}