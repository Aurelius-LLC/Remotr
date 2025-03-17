namespace Remotr.Samples.Calendar;

[RemotrGen]
public class CalendarUpdateEventRecurrenceRule : StatefulCommandHandler<CalendarManagerState, (Guid eventId, RecurrenceRule recurrenceRule), EventState>
{
    public override async Task<EventState> Execute((Guid eventId, RecurrenceRule recurrenceRule) input)
    {
        // Get the event ID
        var eventId = input.eventId;
        
        // Update the event recurrence rule
        var updatedEventState = await CommandFactory.GetChild<EventState>()
            .UpdateRecurrenceRule(input.recurrenceRule)
            .Run(eventId.ToString());
            
        // Update the event in the day state
        await CommandFactory.GetChild<DayState>()
            .EditEventInDay(updatedEventState)
            .Run(updatedEventState.Date.ToString());
            
        return updatedEventState;
    }
} 