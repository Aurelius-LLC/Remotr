using Orleans.Concurrency;
using Orleans.Runtime;

namespace Remotr;

public interface IAggregateRoot : ITransactionalStateFetcher
{
    Task Wait();

    [OneWay]
    Task DeferEntityGrainTransactionCallback<T>(IAggregateEntity<T> entityGrain, Guid callbackId) where T : new();

    [AlwaysInterleave]
    Task NotifyOfTransactionParticipation(Guid transactionId, GrainId grainId, Guid activationId);

    Task<T> ExecuteCommand<T>(ExecutionStep<T> execution);

    [AlwaysInterleave]
    Task<T> ExecuteQuery<T>(ExecutionStep<T> execution, bool forceConsistency);
}