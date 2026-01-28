using Application.Quests.Dtos;
using MediatR;

namespace Application.Quests.Queries.GetQuests;

public record GetQuestsQuery(bool? IsActive = null, string? Search = null) : IRequest<List<QuestDto>>;
