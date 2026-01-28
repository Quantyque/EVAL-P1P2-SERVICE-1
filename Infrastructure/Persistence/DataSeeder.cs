using Domain.Entities;
using Domain.Enums;
using Marten;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Persistence;

/// <summary>
/// Seeds initial data and forces Marten schema creation on startup.
/// </summary>
public class DataSeeder(IServiceProvider serviceProvider, ILogger<DataSeeder> logger) : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = serviceProvider.CreateScope();
        var session = scope.ServiceProvider.GetRequiredService<IDocumentStore>().LightweightSession();

        // Check if any quest exists
        var existing = await session.Query<QuestDefinition>().AnyAsync(cancellationToken);
        if (!existing)
        {
            logger.LogInformation("No quests found. Seeding initial quest to initialize Database Schema...");

            var quest = new QuestDefinition
            {
                Id = Guid.NewGuid(),
                Code = "QUEST-001",
                Title = "The Beginning",
                Description = "Complete your first dungeon.",
                Type = QuestType.DungeonCompletion,
                TargetCount = 1,
                IsActive = true,
                Reward = "100 Gold"
            };

            session.Store(quest);
            await session.SaveChangesAsync(cancellationToken);
            
            logger.LogInformation("Database Schema initialized and initial quest seeded.");
        }
        else
        {
            logger.LogInformation("Database already contains quests. Skipping seed.");
        }
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
