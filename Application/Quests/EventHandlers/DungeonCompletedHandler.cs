using Application.Contracts;
using Domain.Entities;
using Domain.Enums;
using Marten;
using Microsoft.Extensions.Logging;
using Wolverine;

namespace Application.Quests.EventHandlers;

public class DungeonCompletedHandler(IDocumentSession session, ILogger<DungeonCompletedHandler> logger)
{
    public async Task Handle(DungeonCompleted message, CancellationToken cancellationToken)
    {
        logger.LogInformation("Processing DungeonCompleted event {EventId} for player {PlayerId}", message.EventId, message.PlayerId);

        // 1. Find all active dungeon quests
        // Note: In a real high-scale system, we might cache active quests or filter by strict applicability.
        var activeQuests = await session.Query<QuestDefinition>()
            .Where(q => q.IsActive && q.Type == QuestType.DungeonCompletion)
            .ToListAsync(cancellationToken);

        if (!activeQuests.Any())
        {
            logger.LogInformation("No active dungeon quests found.");
            return;
        }

        // 2. Process each quest for the player
        foreach (var quest in activeQuests)
        {
            // Try to find existing player quest state
            // We search by PlayerId and QuestId. Use Query provided by Marten.
            var playerQuest = await session.Query<PlayerQuest>()
                .FirstOrDefaultAsync(pq => pq.PlayerId == message.PlayerId && pq.QuestId == quest.Id, cancellationToken);

            if (playerQuest == null)
            {
                // Auto-start the quest
                playerQuest = new PlayerQuest
                {
                    Id = Guid.NewGuid(),
                    PlayerId = message.PlayerId,
                    QuestId = quest.Id,
                    Status = QuestStatus.NotStarted,
                    ProgressCount = 0,
                    UpdatedAt = DateTime.UtcNow
                };
                session.Store(playerQuest);
                logger.LogInformation("Started quest {QuestTitle} for player {PlayerId}", quest.Title, message.PlayerId);
            }

            // Update progress
            // Store previous status for change logging if needed
            var previousStatus = playerQuest.Status;
            
            playerQuest.AddProgress(1, quest.TargetCount);
            
            if (previousStatus != QuestStatus.Completed && playerQuest.Status == QuestStatus.Completed)
            {
                logger.LogInformation("Player {PlayerId} COMPLETED quest {QuestTitle}!", message.PlayerId, quest.Title);
                // Potential: Publish QuestCompleted event here
            }

            // We update the document in the session
            session.Update(playerQuest);
        }

        // Save all changes in one transaction
        await session.SaveChangesAsync(cancellationToken);
        
        logger.LogInformation("Successfully processed DungeonCompleted event {EventId}", message.EventId);
    }
}
