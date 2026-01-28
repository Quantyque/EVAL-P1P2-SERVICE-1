using Application.Contracts;
using Microsoft.Extensions.Logging;
using Wolverine;

namespace Application.Sagas;

/// <summary>
/// Orchestrated saga that starts a ping, waits for a pong, and times out if no response arrives.
/// Persisted by Marten in Service-1.
/// </summary>
public class PingSaga : Saga
{
    public Guid Id { get; set; }
    public string? Status { get; set; }
    public DateTimeOffset StartedAt { get; set; }
    public DateTimeOffset? CompletedAt { get; set; }
    public string? LastMessage { get; set; }

    /// <summary>
    /// Starts the saga and emits the first ping plus a timeout message.
    /// </summary>
    public static (PingSaga, PingRequested, PingTimeout) Start(StartPingSaga start, ILogger<PingSaga> logger)
    {
        var saga = new PingSaga
        {
            Id = start.SagaId,
            StartedAt = DateTimeOffset.UtcNow,
            Status = "Started",
            LastMessage = start.Message
        };

        logger.LogInformation("Ping saga {SagaId} started. Publishing PingRequested.", saga.Id);

        var ping = new PingRequested(start.SagaId, start.FromService, start.Message);
        var timeout = new PingTimeout(start.SagaId);

        return (saga, ping, timeout);
    }

    /// <summary>
    /// Completes the saga when a pong is received.
    /// </summary>
    public void Handle(PongReturned pong, ILogger<PingSaga> logger)
    {
        if (IsCompleted())
            return;

        Status = "Completed";
        CompletedAt = DateTimeOffset.UtcNow;
        LastMessage = pong.Message;

        logger.LogInformation("Ping saga {SagaId} completed with pong from {FromService}.", Id, pong.FromService);

        MarkCompleted();
    }

    /// <summary>
    /// Marks the saga as timed out when no pong arrives in time.
    /// </summary>
    public void Handle(PingTimeout timeout, ILogger<PingSaga> logger)
    {
        if (IsCompleted())
            return;

        Status = "TimedOut";
        CompletedAt = DateTimeOffset.UtcNow;

        logger.LogWarning("Ping saga {SagaId} timed out.", Id);

        MarkCompleted();
    }

    /// <summary>
    /// Called when a pong arrives but no saga instance exists.
    /// </summary>
    public static void NotFound(PongReturned pong, ILogger<PingSaga> logger)
    {
        logger.LogWarning("PongReturned received for unknown saga {SagaId}.", pong.SagaId);
    }
}

/// <summary>
/// Timeout message scheduled by the saga if no pong is received in time.
/// </summary>
public record PingTimeout(Guid SagaId) : TimeoutMessage(TimeSpan.FromSeconds(20));
