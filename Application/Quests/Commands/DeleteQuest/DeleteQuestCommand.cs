using MediatR;

namespace Application.Quests.Commands.DeleteQuest;

public record DeleteQuestCommand(Guid Id) : IRequest;
