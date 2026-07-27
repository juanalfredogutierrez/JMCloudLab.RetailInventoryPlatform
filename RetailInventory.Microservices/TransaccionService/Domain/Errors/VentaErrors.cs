using BuildingBlocks.Application;

namespace TransaccionService.Domain.Errors;

public static class VentaErrors
{
    public static Error SinItems =>
        new(
            "Venta.SinItems",
            "La venta debe contener al menos un producto");

    public static Error ProductoInvalido =>
        new(
            "Venta.ProductoInvalido",
            "El producto es inválido.");

    public static Error CantidadInvalida =>
        new(
            "Venta.CantidadInvalida",
            "La cantidad debe ser mayor a cero.");

    public static Error PrecioInvalido =>
        new(
            "Venta.PrecioInvalido",
            "El precio debe ser mayor a cero.");

    public static Error StockInsuficiente(
        int productoId,
        int disponible,
        int solicitado)
        => new(
            "Venta.StockInsuficiente",
            $"Producto {productoId}. Disponible: {disponible}, Solicitado: {solicitado}");
}