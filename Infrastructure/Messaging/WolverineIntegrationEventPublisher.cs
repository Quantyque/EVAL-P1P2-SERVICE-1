using Application.Common.Interfaces;
using Wolverine;

namespace Infrastructure.Messaging;

/// <summary>
/// Publishes integration events using Wolverine's message bus.
/// </summary>
public class WolverineIntegrationEventPublisher : IIntegrationEventPublisher
{
    private readonly IMessageBus _bus;

    /// <summary>
    /// Initializes a new instance of the <see cref="WolverineIntegrationEventPublisher"/> class.
    /// </summary>
    public WolverineIntegrationEventPublisher(IMessageBus bus)
    {
        _bus = bus;
    }

    /// <summary>
    /// Publishes an integration event to the Wolverine message bus.
    /// </summary>
    public Task PublishAsync<T>(T message, CancellationToken cancellationToken)
    {
        return _bus.PublishAsync(message).AsTask();
    }
}
