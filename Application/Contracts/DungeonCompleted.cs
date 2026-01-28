namespace Application.Contracts;

public record DungeonCompleted(Guid EventId, string PlayerId, Guid DungeonId, DateTime CompletedAt);
