namespace Remotr;

public interface ISetUpdateState<TState>
    where TState : new()
{
    internal void SetUpdateState(ICanUpdateState<TState> updater);
}