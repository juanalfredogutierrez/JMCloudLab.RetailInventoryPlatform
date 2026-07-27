using ProductoService.DTO;

namespace ProductoService.Application.Queries.GetProductos;

public class GetProductosHandler : IRequestHandler<GetProductosQuery, Result<List<ProductoDto>>>
{
    private readonly ProductoDbContext _context;

    public GetProductosHandler(ProductoDbContext context)
    {
        _context = context;
    }
    public async Task<Result<List<ProductoDto>>> Handle(
        GetProductosQuery request,
        CancellationToken cancellationToken)
    {
        var productos = await _context.Productos
            .Select(p => new ProductoDto(
                p.Id,
                p.Uid,
                p.Codigo,
                p.Nombre,
                p.Precio,
                p.Activo))
            .ToListAsync(cancellationToken);
     

        return Result<List<ProductoDto>>.Success(productos);
    }
}