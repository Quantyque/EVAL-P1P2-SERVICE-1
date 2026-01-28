using MediatR;
using Wolverine;
using Application.Contracts;

namespace Application.Welcome;

public sealed class RequestWelcomeCommandHandler : IRequestHandler<RequestWelcomeCommand, WelcomeResponse>
{
    private readonly IMessageBus _bus;

    /// <summary>
    /// Initializes a new instance of the <see cref="RequestWelcomeCommandHandler"/> class.
    /// </summary>
    public RequestWelcomeCommandHandler(IMessageBus bus)
    {
        _bus = bus;
    }

    /// <summary>
    /// Starts the ping saga and returns a welcome response.
    /// </summary>
    public async Task<WelcomeResponse> Handle(RequestWelcomeCommand request, CancellationToken cancellationToken)
    {
        var sagaId = Guid.NewGuid();
        var message = $"Welcome from {request.ServiceName}. Ping sent.";

        await _bus.InvokeAsync(
            new StartPingSaga(sagaId, request.ServiceName, message),
            cancellationToken);

        return new WelcomeResponse(request.ServiceName, message, sagaId);
    }
}
