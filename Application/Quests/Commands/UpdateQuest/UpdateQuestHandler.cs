using Domain.Entities;
using FluentValidation;
using Marten;
using MediatR;

namespace Application.Quests.Commands.UpdateQuest;

public class UpdateQuestHandler(IDocumentSession session) : IRequestHandler<UpdateQuestCommand>
{
    public async Task Handle(UpdateQuestCommand request, CancellationToken cancellationToken)
    {
        var quest = await session.LoadAsync<QuestDefinition>(request.Id, cancellationToken);
        if (quest == null)
        {
            throw new KeyNotFoundException($"Quest with ID {request.Id} not found.");
        }

        // Check uniqueness of Code if changed
        if (quest.Code != request.Code)
        {
            var existing = await session.Query<QuestDefinition>().FirstOrDefaultAsync(q => q.Code == request.Code, cancellationToken);
            if (existing != null)
            {
                throw new ValidationException($"Quest with code '{request.Code}' already exists.");
            }
        }

        quest.Code = request.Code;
        quest.Title = request.Title;
        quest.Description = request.Description;
        quest.Type = request.Type;
        quest.TargetCount = request.TargetCount;
        quest.IsActive = request.IsActive;
        quest.StartAt = request.StartAt;
        quest.EndAt = request.EndAt;
        quest.Reward = request.Reward;

        session.Update(quest);
        await session.SaveChangesAsync(cancellationToken);
    }
}
