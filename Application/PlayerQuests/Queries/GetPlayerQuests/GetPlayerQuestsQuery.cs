using Application.PlayerQuests.Dtos;
using MediatR;

namespace Application.PlayerQuests.Queries.GetPlayerQuests;

public record GetPlayerQuestsQuery(string PlayerId) : IRequest<List<PlayerQuestDto>>;
