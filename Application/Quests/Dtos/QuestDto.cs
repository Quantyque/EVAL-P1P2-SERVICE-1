using Domain.Enums;

namespace Application.Quests.Dtos;

public record QuestDto(
    Guid Id,
    string Code,
    string Title,
    string? Description,
    QuestType Type,
    int TargetCount,
    bool IsActive,
    DateTime? StartAt,
    DateTime? EndAt,
    string? Reward
);
