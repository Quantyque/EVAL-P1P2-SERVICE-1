using Application.Quests.Commands.CreateQuest;
using Application.Quests.Commands.DeleteQuest;
using Application.Quests.Commands.UpdateQuest;
using Application.Quests.Queries.GetQuestById;
using Application.Quests.Queries.GetQuests;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.Endpoints;

public static class QuestEndpoints
{
    public static void MapQuestEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/quests").WithTags("Quests");

        group.MapGet("/", async (ISender sender, [FromQuery] bool? isActive, [FromQuery] string? search, CancellationToken ct) =>
            await sender.Send(new GetQuestsQuery(isActive, search), ct));

        group.MapGet("/{id:guid}", async (ISender sender, Guid id, CancellationToken ct) =>
        {
            var result = await sender.Send(new GetQuestByIdQuery(id), ct);
            return result is not null ? Results.Ok(result) : Results.NotFound();
        });

        group.MapPost("/", async (ISender sender, [FromBody] CreateQuestCommand command, CancellationToken ct) =>
        {
            var id = await sender.Send(command, ct);
            return Results.Created($"/quests/{id}", id);
        });

        group.MapPut("/{id:guid}", async (ISender sender, Guid id, [FromBody] UpdateQuestCommand command, CancellationToken ct) =>
        {
            if (id != command.Id) return Results.BadRequest("ID mismatch");
            await sender.Send(command, ct);
            return Results.NoContent();
        });

        group.MapDelete("/{id:guid}", async (ISender sender, Guid id, CancellationToken ct) =>
        {
            await sender.Send(new DeleteQuestCommand(id), ct);
            return Results.NoContent();
        });
    }
}
