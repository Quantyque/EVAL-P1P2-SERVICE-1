using Domain.Entities;
using Marten;
using MediatR;

namespace Application.Quests.Commands.DeleteQuest;

public class DeleteQuestHandler(IDocumentSession session) : IRequestHandler<DeleteQuestCommand>
{
    public async Task Handle(DeleteQuestCommand request, CancellationToken cancellationToken)
    {
        // Soft delete could be implemented by setting IsActive = false, but Marten supports hard delete.
        // The requirements say "soft delete acceptable".
        // I will implement Hard Delete via Marten for simplicity as per "Delete" verb, usually Soft Delete is explicit in requirement.
        // Wait, "DELETE /quests/{id} (soft delete acceptable)".
        // I will do Hard Delete on the document for now, it's cleaner for a "Day" project unless "Archive" is needed.
        
        session.Delete<QuestDefinition>(request.Id);
        await session.SaveChangesAsync(cancellationToken);
    }
}
