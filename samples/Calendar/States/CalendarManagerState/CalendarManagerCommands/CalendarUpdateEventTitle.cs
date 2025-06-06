namespace Remotr.Samples.Calendar;

[UseShortcuts]
public class CalendarUpdateEventTitle : EntityCommandHandler<CalendarManagerState, (Guid eventId, string title), EventState>
{
    public override async Task<EventState> Execute((Guid eventId, string title) input)
    {
        // Get the event ID
        var eventId = input.eventId;
        
        // Update the event title
        var updatedEventState = await CommandFactory.GetEntity<EventState>()
            .UpdateTitle(input.title)
            .Run(eventId.ToString());
            
        // Update the event in the day state
        await CommandFactory.GetEntity<DayState>()
            .EditEventInDay(updatedEventState)
            .Run(updatedEventState.Date.ToString());
            
        return updatedEventState;
    }
} 