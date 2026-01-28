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

    // Logic to update progress
    public void AddProgress(int amount, int target)
    {
        if (Status == QuestStatus.Completed || Status == QuestStatus.Claimed) return;

        if (Status == QuestStatus.NotStarted) Status = QuestStatus.InProgress;

        ProgressCount += amount;
        
        if (ProgressCount >= target)
        {
            ProgressCount = target; // Cap at target
            Status = QuestStatus.Completed;
            CompletedAt = DateTime.UtcNow;
        }
        
        UpdatedAt = DateTime.UtcNow;
    }
}
