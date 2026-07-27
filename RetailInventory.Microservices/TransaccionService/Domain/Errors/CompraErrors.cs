namespace TransaccionService.Domain.Errors;

public static class CompraErrors
{
    public static readonly Error SinItems =
        new(
            "Compra.SinItems",
            "La compra debe contener al menos un producto");

    public static readonly Error CantidadInvalida =
        new(
            "Compra.CantidadInvalida",
            "La cantidad debe ser mayor a cero");

    public static readonly Error PrecioInvalido =
        new(
            "Compra.PrecioInvalido",
            "El precio debe ser mayor a cero");

    public static readonly Error ProductoInvalido =
    new(
        "Compra.ProductoInvalido",
        "El producto es inválido.");
}