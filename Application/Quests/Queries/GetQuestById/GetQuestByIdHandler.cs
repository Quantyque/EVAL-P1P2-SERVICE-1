using Application.Quests.Dtos;
using Domain.Entities;
using Mapster;
using Marten;
using MediatR;

namespace Application.Quests.Queries.GetQuestById;

public class GetQuestByIdHandler(IQuerySession session) : IRequestHandler<GetQuestByIdQuery, QuestDto?>
{
    public async Task<QuestDto?> Handle(GetQuestByIdQuery request, CancellationToken cancellationToken)
    {
        var quest = await session.LoadAsync<QuestDefinition>(request.Id, cancellationToken);
        return quest?.Adapt<QuestDto>();
    }
}
