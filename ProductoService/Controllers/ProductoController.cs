using ProductoService.Application.Commands.CreateProducto;
using ProductoService.Application.Commands.UpdateProduct;
using ProductoService.Application.Queries.GetProductos;
using ProductoService.Extensions;


namespace ProductoService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductoController : ControllerBase
{
    private readonly IMediator _mediator;

    public ProductoController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateProductoCommand command)
    {
        var result = await _mediator.Send(command);

        return this.ToActionResult(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _mediator.Send(new GetProductosQuery());

        return this.ToActionResult(result);
    }

    [HttpPut("actualizar-costo")]
    public async Task<IActionResult> ActualizarCosto(
    ActualizarCostoProductoCommand command)
    {
        var result = await _mediator.Send(command);

        return this.ToActionResult(result);
    }
}