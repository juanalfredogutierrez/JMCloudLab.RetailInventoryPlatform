using BuildingBlocks.Application;
using MediatR;

namespace InventarioService.Application.Queries.GetStock
{
    public record GetStockQuery(int ProductoId) : IRequest<Result<int>>;
}
