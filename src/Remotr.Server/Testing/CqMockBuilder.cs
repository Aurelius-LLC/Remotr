namespace Remotr.Testing;

public class CqMockBuilder : ICqMockBuilder
{
    private readonly Dictionary<Type, object> _mocks = new();

    public ICqMockBuilder MockManager<TGrain, TCommand>(IAsyncCommandHandler<TGrain> mock)
        where TGrain : IGrain
        where TCommand : IAsyncCommandHandler<TGrain>
    {
        _mocks.Add(typeof(TCommand), mock);
        return this;
    }

    public ICqMockBuilder MockManager<TGrain, TCommand, TOutput>(IAsyncCommandHandler<TGrain, TOutput> mock)
        where TGrain : IGrain
        where TCommand : IAsyncCommandHandler<TGrain, TOutput>
    {
        _mocks.Add(typeof(TCommand), mock);
        return this;
    }

    public ICqMockBuilder MockManager<TGrain, TCommand, TInput, TOutput>(IAsyncCommandHandler<TGrain, TInput, TOutput> mock)
        where TGrain : IGrain
        where TCommand : IAsyncCommandHandler<TGrain, TInput, TOutput>
    {
        _mocks.Add(typeof(TCommand), mock);
        return this;
    }

    public ICqMockBuilder MockManager<TGrain, TQuery, TOutput>(IAsyncQueryHandler<TGrain, TOutput> mock)
        where TGrain : IGrain
        where TQuery : IAsyncQueryHandler<TGrain, TOutput>
    {
        _mocks.Add(typeof(TQuery), mock);
        return this;
    }

    public ICqMockBuilder MockManager<TGrain, TQuery, TInput, TOutput>(IAsyncQueryHandler<TGrain, TInput, TOutput> mock)
        where TGrain : IGrain
        where TQuery : IAsyncQueryHandler<TGrain, TInput, TOutput>
    {
        _mocks.Add(typeof(TQuery), mock);
        return this;
    }


    public ICqMockBuilder MockChild<TState, TCommand>(IAsyncCommandHandler<IAggregateEntity<TState>> mock)
        where TState : new()
        where TCommand : IAsyncCommandHandler<IAggregateEntity<TState>>
    {
        _mocks.Add(typeof(TCommand), mock);
        return this;
    }

    public ICqMockBuilder MockChild<TState, TCommand, TOutput>(IAsyncCommandHandler<IAggregateEntity<TState>, TOutput> mock)
        where TState : new()
        where TCommand : IAsyncCommandHandler<IAggregateEntity<TState>, TOutput>
    {
        _mocks.Add(typeof(TCommand), mock);
        return this;
    }

    public ICqMockBuilder MockChild<TState, TCommand, TInput, TOutput>(IAsyncCommandHandler<IAggregateEntity<TState>, TInput, TOutput> mock)
        where TState : new()
        where TCommand : IAsyncCommandHandler<IAggregateEntity<TState>, TInput, TOutput>
    {
        _mocks.Add(typeof(TCommand), mock);
        return this;
    }

    public ICqMockBuilder MockChild<TState, TQuery, TOutput>(IAsyncQueryHandler<IAggregateEntity<TState>, TOutput> mock)
        where TState : new()
        where TQuery : IAsyncQueryHandler<IAggregateEntity<TState>, TOutput>
    {
        _mocks.Add(typeof(TQuery), mock);
        return this;
    }

    public ICqMockBuilder MockChild<TState, TQuery, TInput, TOutput>(IAsyncQueryHandler<IAggregateEntity<TState>, TInput, TOutput> mock)
        where TState : new()
        where TQuery : IAsyncQueryHandler<IAggregateEntity<TState>, TInput, TOutput>
    {
        _mocks.Add(typeof(TQuery), mock);
        return this;
    }

    internal Dictionary<Type, object> GetMocks()
    {
        return _mocks;
    }
}

