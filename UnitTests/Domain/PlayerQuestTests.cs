using Domain.Entities;
using Domain.Enums;
using FluentAssertions;

namespace UnitTests.Domain;

public class PlayerQuestTests
{
    [Fact]
    public void AddProgress_Should_IncrementProgress_And_UpdateStatus_When_Not_Completed()
    {
        // Arrange
        var playerQuest = new PlayerQuest
        {
            PlayerId = "player-1",
            QuestId = Guid.NewGuid(),
            Status = QuestStatus.NotStarted,
            ProgressCount = 0,
            UpdatedAt = DateTime.UtcNow
        };
        var targetCount = 3;
        var eventId = Guid.NewGuid();

        // Act
        playerQuest.AddProgress(1, targetCount, eventId);

        // Assert
        playerQuest.ProgressCount.Should().Be(1);
        playerQuest.Status.Should().Be(QuestStatus.InProgress);
        playerQuest.ProcessedEventIds.Should().Contain(eventId);
    }

    [Fact]
    public void AddProgress_Should_CompleteQuest_When_Target_Reached()
    {
        // Arrange
        var playerQuest = new PlayerQuest
        {
            PlayerId = "player-1",
            QuestId = Guid.NewGuid(),
            Status = QuestStatus.InProgress,
            ProgressCount = 2,
            UpdatedAt = DateTime.UtcNow
        };
        var targetCount = 3;
        var eventId = Guid.NewGuid();

        // Act
        playerQuest.AddProgress(1, targetCount, eventId);

        // Assert
        playerQuest.ProgressCount.Should().Be(3);
        playerQuest.Status.Should().Be(QuestStatus.Completed);
        playerQuest.CompletedAt.Should().NotBeNull();
        playerQuest.ProcessedEventIds.Should().Contain(eventId);
    }

    [Fact]
    public void AddProgress_Should_Be_Idempotent_For_Same_EventId()
    {
        // Arrange
        var eventId = Guid.NewGuid();
        var playerQuest = new PlayerQuest
        {
            PlayerId = "player-1",
            QuestId = Guid.NewGuid(),
            Status = QuestStatus.InProgress,
            ProgressCount = 1,
            UpdatedAt = DateTime.UtcNow,
            ProcessedEventIds = new List<Guid> { eventId }
        };
        var targetCount = 3;

        // Act
        playerQuest.AddProgress(1, targetCount, eventId);

        // Assert
        playerQuest.ProgressCount.Should().Be(1); // Should NOT increment
    }
}
