namespace Application.Contracts;

/// <summary>
/// Command message that starts the ping saga within Service-1.
/// </summary>
/// <param name="SagaId">Saga correlation identifier.</param>
/// <param name="FromService">Originating service name.</param>
/// <param name="Message">Initial message to include in the ping.</param>
public record StartPingSaga(Guid SagaId, string FromService, string Message);
