namespace Remotr.Samples.Calendar;

[UseShortcuts]
public class UpdateDate : EntityCommandHandler<EventState, DateOnly, EventState>
{
    public override async Task<EventState> Execute(DateOnly date)
    {
        var currentState = await GetState();
        
        // Create a new state with the updated date
        var updatedState = currentState with { Date = date };
        
        await UpdateState(updatedState);
        
        return await GetState();
    }
} 