using Application.Common.Interfaces;
using Domain.Entities;
using FluentValidation;
using Marten;
using MediatR;

namespace Application.Quests.Commands.CreateQuest;

public class CreateQuestHandler(IDocumentSession session) : IRequestHandler<CreateQuestCommand, Guid>
{
    public async Task<Guid> Handle(CreateQuestCommand request, CancellationToken cancellationToken)
    {
        // Check for uniqueness of Code
        var existing = await session.Query<QuestDefinition>().FirstOrDefaultAsync(q => q.Code == request.Code, cancellationToken);
        if (existing != null)
        {
            throw new ValidationException($"Quest with code '{request.Code}' already exists.");
        }

        var quest = new QuestDefinition
        {
            Id = Guid.NewGuid(),
            Code = request.Code,
            Title = request.Title,
            Description = request.Description,
            Type = request.Type,
            TargetCount = request.TargetCount,
            IsActive = request.IsActive,
            StartAt = request.StartAt,
            EndAt = request.EndAt,
            Reward = request.Reward
        };

        session.Store(quest);
        await session.SaveChangesAsync(cancellationToken);

        return quest.Id;
    }
}
