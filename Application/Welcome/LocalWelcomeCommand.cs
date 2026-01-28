using MediatR;

namespace Application.Welcome;

/// <summary>
/// Command for a local welcome response without messaging.
/// </summary>
/// <param name="ServiceName">Service name.</param>
public record LocalWelcomeCommand(string ServiceName) : IRequest<WelcomeResponse>;
