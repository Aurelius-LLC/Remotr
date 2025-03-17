namespace Remotr.Testing;

public interface ICqMockBuilder
{
    // Command Manager Mocks
    public ICqMockBuilder MockManager<TGrain, TCommand>(IAsyncCommandHandler<TGrain> mock)
        where TGrain : IGrain
        where TCommand : IAsyncCommandHandler<TGrain>;

    public ICqMockBuilder MockManager<TGrain, TCommand, TOutput>(IAsyncCommandHandler<TGrain, TOutput> mock)
        where TGrain : IGrain
        where TCommand : IAsyncCommandHandler<TGrain, TOutput>;

    public ICqMockBuilder MockManager<TGrain, TCommand, TInput, TOutput>(IAsyncCommandHandler<TGrain, TInput, TOutput> mock)
        where TGrain : IGrain
        where TCommand : IAsyncCommandHandler<TGrain, TInput, TOutput>;


    // Query Manager Mocks
    public ICqMockBuilder MockManager<TGrain, TQuery, TOutput>(IAsyncQueryHandler<TGrain, TOutput> mock)
        where TGrain : IGrain
        where TQuery : IAsyncQueryHandler<TGrain, TOutput>;

    public ICqMockBuilder MockManager<TGrain, TQuery, TInput, TOutput>(IAsyncQueryHandler<TGrain, TInput, TOutput> mock)
        where TGrain : IGrain
        where TQuery : IAsyncQueryHandler<TGrain, TInput, TOutput>;


    // Command Child Mocks
    public ICqMockBuilder MockChild<TState, TCommand>(IAsyncCommandHandler<IAggregateEntity<TState>> mock)
        where TState : new()
        where TCommand : IAsyncCommandHandler<IAggregateEntity<TState>>;

    public ICqMockBuilder MockChild<TState, TCommand, TOutput>(IAsyncCommandHandler<IAggregateEntity<TState>, TOutput> mock)
        where TState : new()
        where TCommand : IAsyncCommandHandler<IAggregateEntity<TState>, TOutput>;

    public ICqMockBuilder MockChild<TState, TCommand, TInput, TOutput>(IAsyncCommandHandler<IAggregateEntity<TState>, TInput, TOutput> mock)
        where TState : new()
        where TCommand : IAsyncCommandHandler<IAggregateEntity<TState>, TInput, TOutput>;


    // Query Child Mocks
    public ICqMockBuilder MockChild<TState, TQuery, TOutput>(IAsyncQueryHandler<IAggregateEntity<TState>, TOutput> mock)
        where TState : new()
        where TQuery : IAsyncQueryHandler<IAggregateEntity<TState>, TOutput>;

    public ICqMockBuilder MockChild<TState, TQuery, TInput, TOutput>(IAsyncQueryHandler<IAggregateEntity<TState>, TInput, TOutput> mock)
        where TState : new()
        where TQuery : IAsyncQueryHandler<IAggregateEntity<TState>, TInput, TOutput>;
}

