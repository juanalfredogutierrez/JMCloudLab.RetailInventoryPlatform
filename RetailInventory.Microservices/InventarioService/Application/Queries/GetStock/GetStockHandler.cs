using MediatR;

namespace InventarioService.Application.Queries.GetStock;

public class GetStockHandler : IRequestHandler<GetStockQuery, Result<int>>
{
    private readonly InventarioDbContext _context;

    public GetStockHandler(InventarioDbContext context)
    {
        _context = context;
    }

    public async Task<Result<int>> Handle(GetStockQuery request, CancellationToken cancellationToken)
    {
        var stock = await _context.Existencias
            .FirstOrDefaultAsync(x => x.ProductoId == request.ProductoId);

        return Result<int>.Success(stock?.CantidadDisponible ?? 0);
    }
}