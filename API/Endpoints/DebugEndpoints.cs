using Application.Contracts;
using Microsoft.AspNetCore.Mvc;
using Wolverine;

namespace API.Endpoints;

public static class DebugEndpoints
{
    public static void MapDebugEndpoints(this IEndpointRouteBuilder app)
    {
        // Debug endpoints to simulate external events
        var group = app.MapGroup("/debug").WithTags("Debug / Simulation");

        group.MapPost("/dungeon-completed", async (IMessageBus bus, [FromBody] DungeonCompleted command, CancellationToken ct) =>
        {
            // Simulate the Game service publishing the event
            await bus.PublishAsync(command);
            return Results.Accepted(value: new { message = "Event published" });
        });
    }
}
