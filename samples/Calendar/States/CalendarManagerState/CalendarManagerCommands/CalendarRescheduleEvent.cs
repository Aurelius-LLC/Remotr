namespace Remotr.Samples.Calendar;

[RemotrGen]
public class CalendarRescheduleEvent : StatefulCommandHandler<CalendarManagerState, (Guid eventId, DateOnly newDate), EventState>
{
    public override async Task<EventState> Execute((Guid eventId, DateOnly newDate) input)
    {
        // Get current event state
        var eventId = input.eventId;
        var currentEvent = await CommandFactory.GetChild<EventState>()
            .GetEventState()
            .Run(eventId.ToString());
            
        // Store old date for DayState operations
        var oldDate = currentEvent.Date;
        var newDate = input.newDate;
        
        // Update the event state with the new date
        var updatedEventState = await CommandFactory.GetChild<EventState>()
            .UpdateDate(newDate)
            .Run(eventId.ToString());
        
        // If the date has changed, update the DayState objects
        if (oldDate != newDate)
        {
            // Remove from old day
            await CommandFactory.GetChild<DayState>()
                .RemoveEventFromDay(eventId)
                .Run(oldDate.ToString());
                
            // Add to new day
            await CommandFactory.GetChild<DayState>()
                .AddEventToDay(updatedEventState)
                .Run(newDate.ToString());
        }
        
        // Update reminder dictionary
        var calendarState = await GetState();
        var updatedReminderDict = new Dictionary<DateTime, List<Guid>>(calendarState.DateToEventsWithReminders);
        
        // Remove event from old reminder times
        if (currentEvent.Reminders != null)
        {
            foreach (var reminder in currentEvent.Reminders)
            {
                var oldReminderTime = oldDate.ToDateTime(currentEvent.StartTime) - reminder.Key;
                if (updatedReminderDict.TryGetValue(oldReminderTime, out var eventList))
                {
                    eventList.Remove(eventId);
                    if (eventList.Count == 0)
                    {
                        updatedReminderDict.Remove(oldReminderTime);
                    }
                }
            }
        }
        
        // Add event to new reminder times
        if (updatedEventState.Reminders != null)
        {
            foreach (var reminder in updatedEventState.Reminders)
            {
                var newReminderTime = newDate.ToDateTime(updatedEventState.StartTime) - reminder.Key;
                if (!updatedReminderDict.TryGetValue(newReminderTime, out var eventList))
                {
                    eventList = new List<Guid>();
                    updatedReminderDict[newReminderTime] = eventList;
                }
                
                if (!eventList.Contains(eventId))
                {
                    eventList.Add(eventId);
                }
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