using Orleans.Runtime;

namespace Remotr;

internal sealed record ParticipatingGrainIdStore
{
    private Guid TransactionId { get; set; }

    private IDictionary<GrainId, Guid> ParticipatingGrains { get; } = new Dictionary<GrainId, Guid>();

    public void AddParticipatingGrain(Guid transactionId, GrainId grainId, Guid activationId)
    {
        CheckTransactionId(transactionId);
        var containsGrainId = ParticipatingGrains.TryGetValue(grainId, out Guid value);
        if (containsGrainId && value != activationId)
        {
            throw new InvalidOperationException("Grain has restarted.");
        }
        else
        {
            ParticipatingGrains[grainId] = activationId;
        }
    }

    public IEnumerable<GrainId> GetGrainIds(Guid transactionId)
    {
        CheckTransactionId(transactionId);
        return ParticipatingGrains.Keys;
    }

    private void CheckTransactionId(Guid transactionId)
    {
        if (TransactionId != transactionId)
        {
            TransactionId = transactionId;
            ParticipatingGrains.Clear();
        }
    }
}
