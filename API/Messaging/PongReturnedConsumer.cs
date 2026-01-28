using Application.Contracts;
using Application.Welcome;
using MediatR;

namespace API.Messaging;

/// <summary>
/// Receives PongReturned events and forwards them to the CQRS pipeline.
/// </summary>
public class PongReturnedConsumer
{
    private readonly ISender _sender;
    private readonly ILogger<PongReturnedConsumer> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="PongReturnedConsumer"/> class.
    /// </summary>
    public PongReturnedConsumer(ISender sender, ILogger<PongReturnedConsumer> logger)
    {
        _sender = sender;
        _logger = logger;
    }

    /// <summary>
    /// Handles the incoming pong event.
    /// </summary>
    public async Task Handle(PongReturned message, CancellationToken cancellationToken)
    {
        var response = await _sender.Send(
            new ProcessPongCommand(message.SagaId, message.FromService, message.Message),
            cancellationToken);

        _logger.LogInformation("Pong received from {FromService} ({CorrelationId}): {Message}",
            message.FromService,
            response.CorrelationId,
            response.Message);
    }
}
