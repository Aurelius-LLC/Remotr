namespace Remotr;

public interface ICanUpdateState<TState> where TState : new()
{
    public ValueTask UpdateState(TState newState);
    public ValueTask ClearState();
}