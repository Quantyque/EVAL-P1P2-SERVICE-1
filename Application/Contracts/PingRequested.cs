using Wolverine.Attributes;
using Wolverine.Persistence.Sagas;

namespace Application.Contracts;

/// <summary>
/// Event published by Service-1 to request a pong response from Service-2.
/// </summary>
/// <param name="SagaId">Saga correlation identifier.</param>
/// <param name="FromService">Originating service name.</param>
/// <param name="Message">Human-readable message payload.</param>
[MessageIdentity("eval.ping.requested")]
public record PingRequested([property: SagaIdentity] Guid SagaId, string FromService, string Message);
