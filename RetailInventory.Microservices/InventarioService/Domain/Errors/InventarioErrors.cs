namespace InventarioService.Domain.Errors;

using BuildingBlocks.Application;

public static class InventarioErrors
{
    public static readonly Error StockInsuficiente =
        new(
            "Inventario.StockInsuficiente",
            "No existe stock suficiente para completar la operación.");

    public static readonly Error ProductoNoEncontrado =
        new(
            "Inventario.ProductoNoEncontrado",
            "No existe registro de inventario para el producto.");
}   