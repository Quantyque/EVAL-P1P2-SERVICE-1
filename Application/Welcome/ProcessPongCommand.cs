using MediatR;

namespace Application.Welcome;

/// <summary>
/// Command that processes a pong response in the CQRS pipeline.
/// </summary>
/// <param name="SagaId">Saga correlation identifier.</param>
/// <param name="FromService">Originating service name.</param>
/// <param name="Message">Response message.</param>
public record ProcessPongCommand(Guid SagaId, string FromService, string Message) : IRequest<WelcomeResponse>;
