namespace Application.Welcome;

/// <summary>
/// Response returned by welcome handlers.
/// </summary>
/// <param name="Service">Service name.</param>
/// <param name="Message">Response message.</param>
/// <param name="CorrelationId">Correlation identifier.</param>
public record WelcomeResponse(string Service, string Message, Guid CorrelationId);
