using Orleans.Runtime;

namespace Remotr;

internal interface ISetManagerGrain
{
    public void SetManagerGrain(GrainId managerGrainId);
}
