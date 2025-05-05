namespace Remotr.Samples.Calendar;

[UseShortcuts]
public class CalendarAddEvent : EntityCommandHandler<CalendarManagerState, EventState, EventState>
{
    public override async Task<EventState> Execute(EventState eventState)
    {
        // Create the event
        var createdEvent = await CommandFactory.GetEntity<EventState>()
            .CreateEventState(eventState)
            .Run(eventState.Id.ToString());
            
        // Add the event to the day
        await CommandFactory.GetEntity<DayState>()
            .AddEventToDay(createdEvent)
            .Run(createdEvent.Date.ToString());
            
        // Update the manager state with any reminders
        if (createdEvent.Reminders != null && createdEvent.Reminders.Count > 0)
        {
            var currentState = await GetState();
            var updatedReminderDict = new Dictionary<DateTime, List<Guid>>(currentState.DateToEventsWithReminders);
            
            foreach (var reminder in createdEvent.Reminders)
            {
                var reminderTime = createdEvent.Date.ToDateTime(createdEvent.StartTime) - reminder.Key;
                if (!updatedReminderDict.TryGetValue(reminderTime, out var eventList))
                {
                    eventList = new List<Guid>();
                    updatedReminderDict[reminderTime] = eventList;
                }
                
                eventList.Add(createdEvent.Id);
            }
            
            await UpdateState(new CalendarManagerState
            {
                DateToEventsWithReminders = updatedReminderDict
            });
        }
        
        return createdEvent;
    }
} 