namespace Remotr.Example.Calendar;

[RemotrGen]
public class CalendarUpdateEventTitle : StatefulCommandHandler<CalendarManagerState, (Guid eventId, string title), EventState>
{
    public override async Task<EventState> Execute((Guid eventId, string title) input)
    {
        // Get the event ID
        var eventId = input.eventId;
        
        // Update the event title
        var updatedEventState = await CommandFactory.GetChild<EventState>()
            .UpdateTitle(input.title)
            .Run(eventId.ToString());
            
        // Update the event in the day state
        await CommandFactory.GetChild<DayState>()
            .EditEventInDay(updatedEventState)
            .Run(updatedEventState.Date.ToString());
            
        return updatedEventState;
    }
} 