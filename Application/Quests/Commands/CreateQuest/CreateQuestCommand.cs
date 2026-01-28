using Domain.Enums;
using MediatR;

namespace Application.Quests.Commands.CreateQuest;

public record CreateQuestCommand(
    string Code,
    string Title,
    string? Description,
    QuestType Type,
    int TargetCount,
    bool IsActive,
    DateTime? StartAt,
    DateTime? EndAt,
    string? Reward
) : IRequest<Guid>;
