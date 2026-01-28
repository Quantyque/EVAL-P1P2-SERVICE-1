namespace Application.Common.Interfaces;

/// <summary>
/// Abstraction for publishing integration events from the application layer.
/// </summary>
public interface IIntegrationEventPublisher
{
    /// <summary>
    /// Publishes an integration event.
    /// </summary>
    Task PublishAsync<T>(T message, CancellationToken cancellationToken);
}
