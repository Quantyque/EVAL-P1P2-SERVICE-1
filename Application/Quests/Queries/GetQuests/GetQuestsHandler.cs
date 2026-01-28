using Application.Quests.Dtos;
using Domain.Entities;
using Mapster;
using Marten;
using MediatR;

namespace Application.Quests.Queries.GetQuests;

public class GetQuestsHandler(IQuerySession session) : IRequestHandler<GetQuestsQuery, List<QuestDto>>
{
    public async Task<List<QuestDto>> Handle(GetQuestsQuery request, CancellationToken cancellationToken)
    {
        IQueryable<QuestDefinition> query = session.Query<QuestDefinition>();

        if (request.IsActive.HasValue)
        {
            query = query.Where(q => q.IsActive == request.IsActive.Value);
        }

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var search = request.Search.ToLower();
            query = query.Where(q => q.Code.ToLower().Contains(search) || q.Title.ToLower().Contains(search));
        }

        var results = await query.ToListAsync(cancellationToken);
        
        return results.Adapt<List<QuestDto>>();
    }
}
