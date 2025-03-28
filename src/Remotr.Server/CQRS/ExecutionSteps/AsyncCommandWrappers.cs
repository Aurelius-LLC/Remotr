namespace Remotr;

[GenerateSerializer]
public class AsyncCommandWrapper<IGrainType, Command> : ExecutionStep<object>
    where IGrainType : IGrain
    where Command : IAsyncCommandHandler<IGrainType>
{
    private IAsyncCommandHandler<IGrainType>? command;

    public override void PassCqCreator(ICqCreator creator)
    {
        command = creator.InstantiateCommand<Command, IAsyncCommandHandler<IGrainType>>();
    }

    public override async ValueTask<object> ExecuteStep()
    {
        await command!.Execute();
        return new object();
    }
}


[GenerateSerializer]
public class AsyncCommandWrapper<IGrainType, Command, Output> : ExecutionStep<Output>
    where IGrainType : IGrain
    where Command : IAsyncCommandHandler<IGrainType, Output>
{
    private IAsyncCommandHandler<IGrainType, Output>? command;

    public override void PassCqCreator(ICqCreator creator)
    {
        command ??= creator.InstantiateCommand<Command, IAsyncCommandHandler<IGrainType, Output>>();
    }

    public override async ValueTask<Output> ExecuteStep()
    {
        return await command!.Execute();
    }
}


[GenerateSerializer]
public class AsyncCommandWrapper<IGrainType, Command, Input, Output> : ExecutionStepWithInputAsync<Input, Output>
    where IGrainType : IGrain
    where Command : IAsyncCommandHandler<IGrainType, Input, Output>
{
    private IAsyncCommandHandler<IGrainType, Input, Output>? command;


    public override void PassCqCreator(ICqCreator creator)
    {
        command = creator.InstantiateCommand<Command, IAsyncCommandHandler<IGrainType, Input, Output>>();
    }

    public override async ValueTask<Output> ExecuteStep(Input input)
    {
        return await command!.Execute(input);
    }
}
