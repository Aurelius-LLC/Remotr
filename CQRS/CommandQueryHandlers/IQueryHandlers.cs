namespace Remotr;

public interface IAsyncQueryHandler<IGrainType, Output> : ICanRead where IGrainType : IGrain
{
    Task<Output> Execute();
}

public interface IAsyncQueryHandler<IGrainType, Input, Output> : ICanRead where IGrainType : IGrain
{
    Task<Output> Execute(Input data);
}
