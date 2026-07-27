using InventarioService.Application.Commands.RegistrarEntrada;
using InventarioService.Application.Commands.RegistrarSalida;
using InventarioService.Application.Queries.GetStock;
using InventarioService.Extensions;
using MediatR;

namespace InventarioService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class InventarioController : ControllerBase
{
    private readonly IMediator _mediator;

    public InventarioController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("entrada")]
    public async Task<IActionResult> Entrada(RegistrarEntradaCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    [HttpPost("salida")]
    public async Task<IActionResult> Salida(RegistrarSalidaCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    [HttpGet("stock/{productoId}")]
    public async Task<IActionResult> Stock(int productoId)
    {
        var result = await _mediator.Send(new GetStockQuery(productoId));
        return this.ToActionResult(result);

    }
}