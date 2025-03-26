namespace Remotr.Samples.Calendar;

[UseShortcuts]
public class AddReminder : EntityCommandHandler<EventState, (TimeSpan timeSpan, ScheduledReminder reminder), EventState>
{
    public override async Task<EventState> Execute((TimeSpan timeSpan, ScheduledReminder reminder) input)
    {
        var (timeSpan, reminder) = input;
        
        // Validate the TimeSpan is positive
        if (timeSpan <= TimeSpan.Zero)
        {
            throw new ArgumentException("Reminder time must be positive");
        }
        
        var currentState = await GetState();
        
        // Initialize the Reminders dictionary if it's null
        var reminders = currentState.Reminders ?? new Dictionary<TimeSpan, ScheduledReminder>();
        
        // Check if the TimeSpan is already in the dictionary
        if (reminders.ContainsKey(timeSpan))
        {
            throw new ArgumentException($"A reminder at {timeSpan} already exists");
        }
        
        // Add the reminder to the dictionary
        reminders[timeSpan] = reminder;
        
        // Create a new state with the updated reminders
        var updatedState = currentState with { Reminders = reminders };
        
        await UpdateState(updatedState);
        
        return await GetState();
    }
} 