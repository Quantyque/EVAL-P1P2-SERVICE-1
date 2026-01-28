using Application.PlayerQuests.Dtos;
using Domain.Entities;
using Marten;
using MediatR;

namespace Application.PlayerQuests.Queries.GetPlayerQuests;

public class GetPlayerQuestsHandler(IQuerySession session) : IRequestHandler<GetPlayerQuestsQuery, List<PlayerQuestDto>>
{
    public async Task<List<PlayerQuestDto>> Handle(GetPlayerQuestsQuery request, CancellationToken cancellationToken)
    {
        // We need to join PlayerQuest with QuestDefinition.
        // Marten doesn't support complex joins in the same way SQL does, but we can load references or do it in application.
        // Given the scale, fetching Player quests and then fetching definitions is fine.
        
        var playerQuests = await session.Query<PlayerQuest>()
            .Where(pq => pq.PlayerId == request.PlayerId)
            .ToListAsync(cancellationToken);
            
        if (!playerQuests.Any()) return new List<PlayerQuestDto>();

        var questIds = playerQuests.Select(pq => pq.QuestId).ToList();
        // Load quests using LINQ Contains which Marten translates to IN
        var quests = await session.Query<QuestDefinition>()
            .Where(q => questIds.Contains(q.Id))
            .ToListAsync(cancellationToken);
        
        var questMap = quests.ToDictionary(q => q.Id);

        var result = new List<PlayerQuestDto>();
        foreach (var pq in playerQuests)
        {
            if (questMap.TryGetValue(pq.QuestId, out var quest))
            {
                result.Add(new PlayerQuestDto(
                    pq.QuestId,
                    quest.Title,
                    quest.Description,
                    pq.Status,
                    pq.ProgressCount,
                    quest.TargetCount, // Target from definition
                    pq.CompletedAt,
                    pq.UpdatedAt
                ));
            }
        }

        return result;
    }
}
