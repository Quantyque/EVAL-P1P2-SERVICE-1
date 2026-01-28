using Application.Quests.Dtos;
using MediatR;

namespace Application.Quests.Queries.GetQuestById;

public record GetQuestByIdQuery(Guid Id) : IRequest<QuestDto?>;
