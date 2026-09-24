#pragma warning disable OPENAI001

using Microsoft.Agents.AI;
using OpenAI;
using OpenAI.Responses;

namespace AiAgentService.Agents;

public sealed class RetailInventoryAgent
{
    private readonly AIAgent _agent;

    public RetailInventoryAgent(IConfiguration configuration)
    {
        var apiKey =
            configuration["OpenAI:ApiKey"]
            ?? throw new InvalidOperationException(
                "OpenAI:ApiKey no está configurada.");

        var model =
            configuration["OpenAI:Model"]
            ?? throw new InvalidOperationException(
                "OpenAI:Model no está configurado.");

        var client = new OpenAIClient(apiKey);

        var responsesClient = client.GetResponsesClient();

        _agent = responsesClient.AsAIAgent(
            model: model,
            name: "RetailInventoryAgent",
            instructions:
                """
                Eres RetailInventoryAgent,
                un agente inteligente especializado
                en operaciones de inventario.

                Tu función es ayudar a los usuarios a
                consultar y analizar información del
                sistema RetailInventory.

                Puedes trabajar con información relacionada
                con productos, inventario, ventas y compras.

                Debes responder en español de manera clara,
                profesional y orientada al negocio.

                No inventes datos que no hayan sido
                proporcionados por las herramientas
                disponibles.
                """
        );
    }

    public async Task<string> ExecuteAsync(
        string message,
        CancellationToken cancellationToken)
    {
        var response = await _agent.RunAsync(
            message,
            cancellationToken: cancellationToken);

        return response.ToString();
    }
}

#pragma warning restore OPENAI001