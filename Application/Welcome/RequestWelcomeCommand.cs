using MediatR;

namespace Application.Welcome;

/// <summary>
/// Command that starts the ping saga for the welcome demo.
/// </summary>
/// <param name="ServiceName">Service name.</param>
public record RequestWelcomeCommand(string ServiceName) : IRequest<WelcomeResponse>;
