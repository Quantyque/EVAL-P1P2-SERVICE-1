using Domain.Enums;
using MediatR;

namespace Application.Quests.Commands.UpdateQuest;

public record UpdateQuestCommand(
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
) : IRequest;
