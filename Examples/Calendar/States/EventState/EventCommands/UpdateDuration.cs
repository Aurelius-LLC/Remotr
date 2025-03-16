namespace Remotr.Example.Calendar;

[RemotrGen]
public class UpdateDuration : StatefulCommandHandler<EventState, TimeOnly, EventState>
{
    public override async Task<EventState> Execute(TimeOnly duration)
    {
        var currentState = await GetState();
        
        // Create a new state with the updated duration
        var updatedState = currentState with { Duration = duration };
        
        await UpdateState(updatedState);
        
        return await GetState();
    }
} 