namespace Remotr.Samples.Calendar;

[RemotrGen]
public class UpdateStartTime : StatefulCommandHandler<EventState, TimeOnly, EventState>
{
    public override async Task<EventState> Execute(TimeOnly startTime)
    {
        var currentState = await GetState();
        
        // Create a new state with the updated start time
        var updatedState = currentState with { StartTime = startTime };
        
        await UpdateState(updatedState);
        
        return await GetState();
    }
} 