namespace Remotr.Samples.Calendar;

[RemotrGen]
public class RemoveReminder : EntityCommandHandler<EventState, TimeSpan, EventState>
{
    public override async Task<EventState> Execute(TimeSpan timeSpan)
    {
        var currentState = await GetState();
        
        // Validate that the Reminders dictionary is not null and contains the specified TimeSpan
        if (currentState.Reminders == null)
        {
            throw new ArgumentException("No reminders exist for this event");
        }
        
        if (!currentState.Reminders.ContainsKey(timeSpan))
        {
            throw new ArgumentException($"No reminder exists at {timeSpan}");
        }
        
        // Create a copy of the reminders dictionary
        var reminders = new Dictionary<TimeSpan, ScheduledReminder>(currentState.Reminders);
        
        // Remove the reminder from the dictionary
        reminders.Remove(timeSpan);
        
        // Create a new state with the updated reminders
        var updatedState = currentState with { 
            Reminders = reminders.Count > 0 ? reminders : null 
        };
        
        await UpdateState(updatedState);
        
        return await GetState();
    }
} 