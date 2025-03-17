using Orleans.Runtime;

namespace Remotr;

internal interface ISetAggregate
{
    public void SetAggregate(GrainId aggregateId);
}
