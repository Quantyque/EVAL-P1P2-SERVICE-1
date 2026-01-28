using Application.Welcome;
using MediatR;

namespace API.Endpoints;

/// <summary>
/// Evaluation demo endpoints (local CQRS + RabbitMQ).
/// </summary>
public static class EvaluationEndpoints
{
    /// <summary>
    /// Maps endpoints for Service-1.
    /// </summary>
    public static void MapEvaluationEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/");

        group.MapGet("/", () => Results.Ok(new { service = "EVAL-P1P2-SERVICE-1", status = "ok" }))
            .WithOpenApi(operation => { operation.Summary = "Service-1 liveness endpoint."; return operation; });

        group.MapGet("/welcome", async (ISender sender, CancellationToken ct) =>
                await sender.Send(new LocalWelcomeCommand("EVAL-P1P2-SERVICE-1"), ct))
            .WithOpenApi(operation => { operation.Summary = "CQRS local demo (no RabbitMQ)."; return operation; });

        group.MapGet("/ping", async (ISender sender, CancellationToken ct) =>
                await sender.Send(new RequestWelcomeCommand("EVAL-P1P2-SERVICE-1"), ct))
            .WithOpenApi(operation => { operation.Summary = "CQRS + RabbitMQ demo (starts ping saga)."; return operation; });
    }
}
