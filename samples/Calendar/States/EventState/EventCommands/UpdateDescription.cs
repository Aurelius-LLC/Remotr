namespace Remotr.Samples.Calendar;

[RemotrGen]
public class UpdateDescription : EntityCommandHandler<EventState, string?, EventState>
{
    public override async Task<EventState> Execute(string? description)
    {
        var currentState = await GetState();
        
        // Create a new state with the updated description
        var updatedState = currentState with { Description = description };
        
        await UpdateState(updatedState);
        
        return await GetState();
    }
} 