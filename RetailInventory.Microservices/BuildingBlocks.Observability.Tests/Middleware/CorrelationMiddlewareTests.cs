using BuildingBlocks.Observability.Constants;
using BuildingBlocks.Observability.Middleware;
using FluentAssertions;
using Microsoft.AspNetCore.Http;

namespace BuildingBlocks.Observability.Tests.Middleware;

public class CorrelationMiddlewareTests
{
    [Fact]
    public async Task Should_Generate_CorrelationId_When_Header_Is_Missing()
    {
        // Arrange
        var context = new DefaultHttpContext();

        var middleware = new CorrelationMiddleware(_ => Task.CompletedTask);

        // Act
        await middleware.Invoke(context);

        // Assert
        context.TraceIdentifier.Should().NotBeNullOrWhiteSpace();

        context.Response.Headers
            .Should()
            .ContainKey(HeaderNames.CorrelationId);

        context.Response.Headers[HeaderNames.CorrelationId]
            .ToString()
            .Should()
            .Be(context.TraceIdentifier);
    }

    [Fact]
    public async Task Should_Reuse_CorrelationId_From_Request_Header()
    {
        // Arrange
        var context = new DefaultHttpContext();

        context.Request.Headers[HeaderNames.CorrelationId] = "abc123";

        var middleware = new CorrelationMiddleware(_ => Task.CompletedTask);

        // Act
        await middleware.Invoke(context);

        // Assert
        context.TraceIdentifier.Should().Be("abc123");

        context.Response.Headers[HeaderNames.CorrelationId]
            .ToString()
            .Should()
            .Be("abc123");
    }

    [Fact]
    public async Task Should_Add_CorrelationId_To_Response_Header()
    {
        // Arrange
        var context = new DefaultHttpContext();

        var middleware = new CorrelationMiddleware(_ => Task.CompletedTask);

        // Act
        await middleware.Invoke(context);

        // Assert
        context.Response.Headers
            .Should()
            .ContainKey(HeaderNames.CorrelationId);
    }

    [Fact]
    public async Task Should_Invoke_Next_Middleware()
    {
        // Arrange
        var nextWasCalled = false;

        RequestDelegate next = context =>
        {
            nextWasCalled = true;
            return Task.CompletedTask;
        };

        var middleware = new CorrelationMiddleware(next);

        var httpContext = new DefaultHttpContext();

        // Act
        await middleware.Invoke(httpContext);

        // Assert
        nextWasCalled.Should().BeTrue();
    }
}