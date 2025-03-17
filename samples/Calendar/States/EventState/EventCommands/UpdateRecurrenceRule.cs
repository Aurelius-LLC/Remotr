namespace Remotr.Samples.Calendar;

[RemotrGen]
public class UpdateRecurrenceRule : StatefulCommandHandler<EventState, RecurrenceRule?, EventState>
{
    public override async Task<EventState> Execute(RecurrenceRule? recurrenceRule)
    {
        if (recurrenceRule != null)
        {
            // Validate the RecurrenceRule properties
            if (string.IsNullOrEmpty(recurrenceRule.Frequency))
            {
                throw new ArgumentException("Frequency cannot be null or empty");
            }
            
            if (recurrenceRule.Interval <= 0)
            {
                throw new ArgumentException("Interval must be greater than zero");
            }
        }
        
        var currentState = await GetState();
        
        // Create a new state with the updated recurrence rule
        // If a RecurrenceRule is provided, also set IsRecurring to true
        var updatedState = currentState with { 
            RecurrenceRule = recurrenceRule,
            IsRecurring = recurrenceRule != null || currentState.IsRecurring
        };
        
        await UpdateState(updatedState);
        
        return await GetState();
    }
} 