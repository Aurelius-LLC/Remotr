namespace Remotr.Example.Calendar;

[RemotrGen]
public class CalendarUpdateEventRecurring : StatefulCommandHandler<CalendarManagerState, (Guid eventId, bool isRecurring), EventState>
{
    public override async Task<EventState> Execute((Guid eventId, bool isRecurring) input)
    {
        // Get the event ID
        var eventId = input.eventId;
        
        // Update the event recurring property
        var updatedEventState = await CommandFactory.GetChild<EventState>()
            .UpdateIsRecurring(input.isRecurring)
            .Run(eventId.ToString());
            
        // Update the event in the day state
        await CommandFactory.GetChild<DayState>()
            .EditEventInDay(updatedEventState)
            .Run(updatedEventState.Date.ToString());
            
        return updatedEventState;
    }
} 