namespace Remotr.Samples.Calendar;

[UseShortcuts]
public class CalendarUpdateEventTime : EntityCommandHandler<CalendarManagerState, (Guid eventId, TimeOnly startTime), EventState>
{
    public override async Task<EventState> Execute((Guid eventId, TimeOnly startTime) input)
    {
        // Get the event ID and current state
        var eventId = input.eventId;
        var currentEvent = await CommandFactory.GetEntity<EventState>()
            .GetEventState()
            .Run(eventId.ToString());
            
        // Update the event start time
        var updatedEventState = await CommandFactory.GetEntity<EventState>()
            .UpdateStartTime(input.startTime)
            .Run(eventId.ToString());
            
        // Update the event in the day state
        await CommandFactory.GetEntity<DayState>()
            .EditEventInDay(updatedEventState)
            .Run(updatedEventState.Date.ToString());
            
        // Update reminder dictionary if there are reminders
        if (updatedEventState.Reminders != null && updatedEventState.Reminders.Count > 0)
        {
            var calendarState = await GetState();
            var updatedReminderDict = new Dictionary<DateTime, List<Guid>>(calendarState.DateToEventsWithReminders);
            
            // Remove event from old reminder times
            if (currentEvent.Reminders != null)
            {
                foreach (var reminder in currentEvent.Reminders)
                {
                    var oldReminderTime = currentEvent.Date.ToDateTime(currentEvent.StartTime) - reminder.Key;
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
            foreach (var reminder in updatedEventState.Reminders)
            {
                var newReminderTime = updatedEventState.Date.ToDateTime(updatedEventState.StartTime) - reminder.Key;
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
            
            // Update calendar manager state
            await UpdateState(new CalendarManagerState
            {
                DateToEventsWithReminders = updatedReminderDict
            });
        }
        
        return updatedEventState;
    }
} 