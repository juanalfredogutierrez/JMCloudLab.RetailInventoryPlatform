using TransaccionService.Application.Commands.CreateCompra;
using TransaccionService.Application.Commands.CreateVenta;
using TransaccionService.Extensions;

namespace TransaccionService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TransaccionController : ControllerBase
{
    private readonly IMediator _mediator;

    public TransaccionController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("compra")]
    public async Task<IActionResult> CreateCompra(CreateCompraCommand command)
    {
        var result = await _mediator.Send(command);
        return this.ToActionResult(result);
    }

    [HttpPost("venta")]
    public async Task<IActionResult> CreateVenta(CreateVentaCommand command)
    {
        var result = await _mediator.Send(command);
        return this.ToActionResult(result);
    }
}