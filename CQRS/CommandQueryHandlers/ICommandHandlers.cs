namespace Remotr;

public interface IAsyncCommandHandler<IGrainType> : ICanReadAndWrite where IGrainType : IGrain
{
    Task Execute();
}

public interface IAsyncCommandHandler<IGrainType, Output> : ICanReadAndWrite where IGrainType : IGrain
{
    Task<Output> Execute();
}

public interface IAsyncCommandHandler<IGrainType, Input, Output> : ICanReadAndWrite where IGrainType : IGrain
{
    Task<Output> Execute(Input data);
}

