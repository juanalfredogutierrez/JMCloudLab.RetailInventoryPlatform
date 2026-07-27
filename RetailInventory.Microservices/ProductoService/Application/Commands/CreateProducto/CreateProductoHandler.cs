using ProductoService.Domain.Entities;

namespace ProductoService.Application.Commands.CreateProducto;

public class CreateProductoHandler : IRequestHandler<CreateProductoCommand, Result<Guid>>
{
    private readonly ProductoDbContext _context;
    private readonly ILogger<CreateProductoHandler> _logger;

    public CreateProductoHandler(
        ProductoDbContext context,
        ILogger<CreateProductoHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<Result<Guid>> Handle(CreateProductoCommand request, CancellationToken cancellationToken)
    {
        _logger.LogBusiness("Creando producto");

        var existe = await _context.Productos.AnyAsync(x => x.Codigo == request.Codigo,cancellationToken);

        if (existe)
        {
            return Result<Guid>.Failure(
                Errors.Conflict("Ya existe un producto con ese código."));
        }

        var producto = new Producto
        {
            Codigo = request.Codigo,
            Nombre = request.Nombre,
            Descripcion = request.Descripcion,
            Precio = request.Precio,
            Activo = true,
            CreatedBy = "system"
        };

        _context.Productos.Add(producto);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogBusiness($"Producto creado: {producto.Codigo}");

        return producto.Uid;
    }
}