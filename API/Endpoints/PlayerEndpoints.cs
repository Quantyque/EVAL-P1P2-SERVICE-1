using Application.PlayerQuests.Queries.GetPlayerQuests;
using MediatR;

namespace API.Endpoints;

public static class PlayerEndpoints
{
    public static void MapPlayerEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/players").WithTags("Player Quests");

        group.MapGet("/{playerId}/quests", async (ISender sender, string playerId, CancellationToken ct) =>
            await sender.Send(new GetPlayerQuestsQuery(playerId), ct));
    }
}
