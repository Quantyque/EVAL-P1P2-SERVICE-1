using Domain.Enums;

namespace Application.PlayerQuests.Dtos;

public record PlayerQuestDto(
    Guid QuestId,
    string QuestTitle,
    string? QuestDescription,
    QuestStatus Status,
    int ProgressCount,
    int TargetCount,
    DateTime? CompletedAt,
    DateTime UpdatedAt
);
