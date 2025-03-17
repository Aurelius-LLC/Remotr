namespace Remotr.Samples.Calendar;

[RemotrGen]
public class CalendarAddEventReminder : StatefulCommandHandler<CalendarManagerState, (Guid eventId, KeyValuePair<TimeSpan, ScheduledReminder> reminder), EventState>
{
    public override async Task<EventState> Execute((Guid eventId, KeyValuePair<TimeSpan, ScheduledReminder> reminder) input)
    {
        // Get the event ID
        var eventId = input.eventId;
        
        // Extract the reminder to add
        var reminderToAdd = input.reminder;
        
        // Add the reminder to the event
        var updatedEventState = await CommandFactory.GetEntity<EventState>()
            .AddReminder((reminderToAdd.Key, reminderToAdd.Value))
            .Run(eventId.ToString());
            
        // Update the event in the day state
        await CommandFactory.GetEntity<DayState>()
            .EditEventInDay(updatedEventState)
            .Run(updatedEventState.Date.ToString());
            
        // Update reminder dictionary
        var calendarState = await GetState();
        var updatedReminderDict = new Dictionary<DateTime, List<Guid>>(calendarState.DateToEventsWithReminders);
        
        // Add event to new reminder time
        var reminderTime = updatedEventState.Date.ToDateTime(updatedEventState.StartTime) - reminderToAdd.Key;
        if (!updatedReminderDict.TryGetValue(reminderTime, out var eventList))
        {
            eventList = new List<Guid>();
            updatedReminderDict[reminderTime] = eventList;
        }
        
        if (!eventList.Contains(eventId))
        {
            eventList.Add(eventId);
        }
        
        // Update calendar manager state
        await UpdateState(new CalendarManagerState
        {
            DateToEventsWithReminders = updatedReminderDict
        });
        
        return updatedEventState;
    }
} 