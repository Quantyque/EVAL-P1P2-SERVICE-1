using Domain.Enums;

namespace Domain.Entities;

public class QuestDefinition
{
    public Guid Id { get; set; }
    public required string Code { get; set; }
    public required string Title { get; set; }
    public string? Description { get; set; }
    public QuestType Type { get; set; }
    public int TargetCount { get; set; }
    public bool IsActive { get; set; }
    public DateTime? StartAt { get; set; }
    public DateTime? EndAt { get; set; }
    public string? Reward { get; set; }
    
    // Marten works well with internal logic methods if we want to enrich the domain later
}
