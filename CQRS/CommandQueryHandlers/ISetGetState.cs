namespace Remotr;

public interface ISetGetState<TState>
    where TState : new()
{
    internal void SetGetState(ICanReadState<TState> reader);
}