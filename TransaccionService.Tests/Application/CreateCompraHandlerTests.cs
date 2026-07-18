using BuildingBlocks.Observability.Services;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using System.Net;
using TransaccionService.Application.Commands.CreateCompra;
using TransaccionService.Infrastructure.Persistence;
using TransaccionService.Tests.Helpers;

namespace TransaccionService.Tests.Application;

public class CreateCompraHandlerTests
{
    private readonly TransaccionDbContext _context;
    private readonly Mock<IHttpClientFactory> _httpClientFactoryMock;
    private readonly Mock<ILogger<CreateCompraHandler>> _loggerMock;
    private readonly Mock<ICorrelationContext> _correlationContextMock;
    private readonly CreateCompraHandler _handler;

    public CreateCompraHandlerTests()
    {
        var options = new DbContextOptionsBuilder<TransaccionDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _context = new TransaccionDbContext(options);

        _loggerMock = new Mock<ILogger<CreateCompraHandler>>();
        _httpClientFactoryMock = new Mock<IHttpClientFactory>();
        _correlationContextMock = new Mock<ICorrelationContext>();

        _correlationContextMock
            .Setup(x => x.CorrelationId)
            .Returns("test-correlation-id");

        var httpClient = new HttpClient(
            new FakeHttpMessageHandler(() =>
                new HttpResponseMessage(HttpStatusCode.OK)))
        {
            BaseAddress = new Uri("http://localhost")
        };

        _httpClientFactoryMock
            .Setup(x => x.CreateClient("ProductoApi"))
            .Returns(httpClient);

        _handler = new CreateCompraHandler(
            _context,
            _loggerMock.Object,
            _httpClientFactoryMock.Object,
            _correlationContextMock.Object);
    }

    [Fact]
    public async Task Handle_Should_Create_Compra_When_Request_Is_Valid()
    {
        // Arrange
        var command = new CreateCompraCommand(
            new()
            {
                new DetalleCompraDto(1, 2, 100),
                new DetalleCompraDto(2, 1, 50)
            },
            "Compra de prueba");

        // Act
        var result = await _handler.Handle(
            command,
            CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBe(Guid.Empty);

        var compra = await _context.Compras
            .Include(x => x.Detalles)
            .FirstOrDefaultAsync();

        compra.Should().NotBeNull();
        compra!.TotalCompra.Should().Be(250);
        compra.Detalles.Should().HaveCount(2);

        var outboxMessages = await _context.OutboxMessages.ToListAsync();

        outboxMessages.Should().HaveCount(1);

        var outboxMessage = outboxMessages.Single();

        outboxMessage.EventType.Should().Be("compra.registrada");
        outboxMessage.ProcessedOn.Should().BeNull();
        outboxMessage.Payload.Should().NotBeNullOrWhiteSpace();
    }
}