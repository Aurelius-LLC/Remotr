namespace Remotr.Samples.Calendar;

[UseShortcuts]
public class UpdateIsRecurring : EntityCommandHandler<EventState, bool, EventState>
{
    public override async Task<EventState> Execute(bool isRecurring)
    {
        var currentState = await GetState();
        
        // Create a new state with the updated isRecurring value
        // If isRecurring is set to false, also set RecurrenceRule to null
        var updatedState = currentState with { 
            IsRecurring = isRecurring,
            RecurrenceRule = isRecurring ? currentState.RecurrenceRule : null
        };
        
        await UpdateState(updatedState);
        
        return await GetState();
    }
} 