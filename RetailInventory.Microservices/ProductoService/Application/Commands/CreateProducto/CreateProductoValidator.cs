using FluentValidation;

namespace ProductoService.Application.Commands.CreateProducto;

public class CreateProductoValidator : AbstractValidator<CreateProductoCommand>
{
    public CreateProductoValidator()
    {
        RuleFor(x => x.Codigo)
            .NotEmpty()
            .WithMessage("El código es obligatorio.")
            .MaximumLength(50);

        RuleFor(x => x.Nombre)
            .NotEmpty()
            .WithMessage("El nombre es obligatorio.")
            .MaximumLength(150);

        RuleFor(x => x.Precio)
            .GreaterThan(0)
            .WithMessage("El precio debe ser mayor que cero.");
    }
}