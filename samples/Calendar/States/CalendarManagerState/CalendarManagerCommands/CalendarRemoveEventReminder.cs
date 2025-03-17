namespace Remotr.Samples.Calendar;

[RemotrGen]
public class CalendarRemoveEventReminder : StatefulCommandHandler<CalendarManagerState, (Guid eventId, TimeSpan reminderTimeSpan), EventState>
{
    public override async Task<EventState> Execute((Guid eventId, TimeSpan reminderTimeSpan) input)
    {
        // Get the event ID
        var eventId = input.eventId;
        
        // Extract the reminder TimeSpan to remove
        var reminderTimeSpan = input.reminderTimeSpan;
        
        // Get current event state to know the reminder time
        var currentEvent = await CommandFactory.GetChild<EventState>()
            .GetEventState()
            .Run(eventId.ToString());
            
        // Remove the reminder from the event
        var updatedEventState = await CommandFactory.GetChild<EventState>()
            .RemoveReminder(reminderTimeSpan)
            .Run(eventId.ToString());
            
        // Update the event in the day state
        await CommandFactory.GetChild<DayState>()
            .EditEventInDay(updatedEventState)
            .Run(updatedEventState.Date.ToString());
            
        // Update reminder dictionary
        var calendarState = await GetState();
        var updatedReminderDict = new Dictionary<DateTime, List<Guid>>(calendarState.DateToEventsWithReminders);
        
        // Remove event from the reminder time
        var reminderTime = currentEvent.Date.ToDateTime(currentEvent.StartTime) - reminderTimeSpan;
        if (updatedReminderDict.TryGetValue(reminderTime, out var eventList))
        {
            eventList.Remove(eventId);
            if (eventList.Count == 0)
            {
                updatedReminderDict.Remove(reminderTime);
            }
        }
        
        // Update calendar manager state
        await UpdateState(new CalendarManagerState
        {
            DateToEventsWithReminders = updatedReminderDict
        });
        
        return updatedEventState;
    }
} 