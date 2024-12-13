namespace Remotr;

public interface ICanReadState<TState> where TState : new()
{
    public ValueTask<TState> GetState();
}