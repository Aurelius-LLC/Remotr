﻿namespace Remotr.Testing;

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
    public ICqMockBuilder MockChild<TState, TCommand>(IAsyncCommandHandler<ITransactionChildGrain<TState>> mock)
        where TState : new()
        where TCommand : IAsyncCommandHandler<ITransactionChildGrain<TState>>;

    public ICqMockBuilder MockChild<TState, TCommand, TOutput>(IAsyncCommandHandler<ITransactionChildGrain<TState>, TOutput> mock)
        where TState : new()
        where TCommand : IAsyncCommandHandler<ITransactionChildGrain<TState>, TOutput>;

    public ICqMockBuilder MockChild<TState, TCommand, TInput, TOutput>(IAsyncCommandHandler<ITransactionChildGrain<TState>, TInput, TOutput> mock)
        where TState : new()
        where TCommand : IAsyncCommandHandler<ITransactionChildGrain<TState>, TInput, TOutput>;


    // Query Child Mocks
    public ICqMockBuilder MockChild<TState, TQuery, TOutput>(IAsyncQueryHandler<ITransactionChildGrain<TState>, TOutput> mock)
        where TState : new()
        where TQuery : IAsyncQueryHandler<ITransactionChildGrain<TState>, TOutput>;

    public ICqMockBuilder MockChild<TState, TQuery, TInput, TOutput>(IAsyncQueryHandler<ITransactionChildGrain<TState>, TInput, TOutput> mock)
        where TState : new()
        where TQuery : IAsyncQueryHandler<ITransactionChildGrain<TState>, TInput, TOutput>;
}

