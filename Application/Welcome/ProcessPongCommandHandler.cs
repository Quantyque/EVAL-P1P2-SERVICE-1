using MediatR;

namespace Application.Welcome;

public sealed class ProcessPongCommandHandler : IRequestHandler<ProcessPongCommand, WelcomeResponse>
{
    /// <summary>
    /// Handles a pong response and returns a welcome response.
    /// </summary>
    public Task<WelcomeResponse> Handle(ProcessPongCommand request, CancellationToken cancellationToken)
    {
        var response = new WelcomeResponse("EVAL-P1P2-SERVICE-1", request.Message, request.SagaId);
        return Task.FromResult(response);
    }
}
