using Orleans.Concurrency;
using Orleans.Runtime;

namespace Remotr;

public interface ITransactionManagerGrain : ITransactionalStateFetcher
{
    Task Wait();

    [OneWay]
    Task DeferChildGrainTransactionCallback<T>(ITransactionChildGrain<T> childGrain, Guid callbackId) where T : new();

    [AlwaysInterleave]
    Task NotifyOfTransactionParticipation(Guid transactionId, GrainId grainId, Guid activationId);

    Task<T> ExecuteCommand<T>(ExecutionStep<T> execution);

    [AlwaysInterleave]
    Task<T> ExecuteQuery<T>(ExecutionStep<T> execution, bool forceConsistency);
}