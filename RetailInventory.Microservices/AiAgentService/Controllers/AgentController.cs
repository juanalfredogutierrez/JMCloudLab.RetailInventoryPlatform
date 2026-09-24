using AiAgentService.Agents;
using Microsoft.AspNetCore.Mvc;

namespace AiAgentService.Controllers;

[ApiController]
[Route("api/agent")]
public sealed class AgentController : ControllerBase
{
    private readonly RetailInventoryAgent _agent;

    public AgentController(RetailInventoryAgent agent)
    {
        _agent = agent;
    }

    [HttpPost("chat")]
    public async Task<IActionResult> Chat(
        [FromBody] AgentRequest request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Message))
        {
            return BadRequest(new
            {
                message = "El mensaje es obligatorio."
            });
        }

        var response = await _agent.ExecuteAsync(
            request.Message,
            cancellationToken);

        return Ok(new
        {
            response
        });
    }
}

public sealed record AgentRequest(string Message);