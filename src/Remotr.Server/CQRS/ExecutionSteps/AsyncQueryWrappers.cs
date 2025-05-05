namespace Remotr;

[GenerateSerializer]
public class AsyncQueryWrapper<IGrainType, Query, Output> : ExecutionStep<Output>
    where IGrainType : IGrain
    where Query : IAsyncQueryHandler<IGrainType, Output>
{
    private IAsyncQueryHandler<IGrainType, Output>? query;

    public override void PassCqCreator(ICqCreator creator)
    {
        query ??= creator.InstantiateQuery<Query, IAsyncQueryHandler<IGrainType, Output>>();
    }

    public override async ValueTask<Output> ExecuteStep(bool useCache = true)
    {
        return await query!.Execute();
    }
}

[GenerateSerializer]
public class AsyncQueryWrapper<IGrainType, Query, Input, Output> : ExecutionStepWithInputAsync<Input, Output>
    where IGrainType : IGrain
    where Query : IAsyncQueryHandler<IGrainType, Input, Output>
{
    private IAsyncQueryHandler<IGrainType, Input, Output>? query;

    public override void PassCqCreator(ICqCreator creator)
    {
        query = creator.InstantiateQuery<Query, IAsyncQueryHandler<IGrainType, Input, Output>>();
    }

    public override async ValueTask<Output> ExecuteStep(Input input, bool useCache = true)
    {
        return await query!.Execute(input);
    }
}
