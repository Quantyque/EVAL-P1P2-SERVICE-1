using Domain.Enums;

namespace Domain.Entities;

public class PlayerQuest
{
    public Guid Id { get; set; } // Marten needs an ID for the document itself
    public required string PlayerId { get; set; }
    public Guid QuestId { get; set; }
    public QuestStatus Status { get; set; }
    public int ProgressCount { get; set; }
    public DateTime? CompletedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    // For Idempotency: Track processed event IDs
    public List<Guid> ProcessedEventIds { get; set; } = new();

    // Logic to update progress
    public void AddProgress(int amount, int target, Guid eventId)
    {
        // Business Rule: Idempotency
        if (ProcessedEventIds.Contains(eventId)) return;

        if (Status == QuestStatus.Completed || Status == QuestStatus.Claimed) return;

        if (Status == QuestStatus.NotStarted) Status = QuestStatus.InProgress;

        ProgressCount += amount;
        
        // Track the event
        ProcessedEventIds.Add(eventId);
        
        if (ProgressCount >= target)
        {
            ProgressCount = target; // Cap at target
            Status = QuestStatus.Completed;
            CompletedAt = DateTime.UtcNow;
        }
        
        UpdatedAt = DateTime.UtcNow;
    }
}
