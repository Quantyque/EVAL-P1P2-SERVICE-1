using MediatR;

namespace Application.Welcome;

public sealed class LocalWelcomeCommandHandler : IRequestHandler<LocalWelcomeCommand, WelcomeResponse>
{
    /// <summary>
    /// Handles a local welcome request without messaging.
    /// </summary>
    public Task<WelcomeResponse> Handle(LocalWelcomeCommand request, CancellationToken cancellationToken)
    {
        var response = new WelcomeResponse(
            request.ServiceName,
            $"Welcome from {request.ServiceName}.",
            Guid.NewGuid());

        return Task.FromResult(response);
    }
}
