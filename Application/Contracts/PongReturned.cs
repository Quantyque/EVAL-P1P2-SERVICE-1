using Wolverine.Attributes;
using Wolverine.Persistence.Sagas;

namespace Application.Contracts;

/// <summary>
/// Event published by Service-2 in response to a ping request.
/// </summary>
/// <param name="SagaId">Saga correlation identifier.</param>
/// <param name="FromService">Originating service name.</param>
/// <param name="Message">Human-readable response message.</param>
[MessageIdentity("eval.pong.returned")]
public record PongReturned([property: SagaIdentity] Guid SagaId, string FromService, string Message);
