namespace Remotr.Samples.Calendar;

[UseShortcuts]
public class CalendarUpdateEventDescription : EntityCommandHandler<CalendarManagerState, (Guid eventId, string description), EventState>
{
    public override async Task<EventState> Execute((Guid eventId, string description) input)
    {
        // Get the event ID
        var eventId = input.eventId;
        
        // Update the event description
        var updatedEventState = await CommandFactory.GetEntity<EventState>()
            .UpdateDescription(input.description)
            .Run(eventId.ToString());
            
        // Update the event in the day state
        await CommandFactory.GetEntity<DayState>()
            .EditEventInDay(updatedEventState)
            .Run(updatedEventState.Date.ToString());
            
        return updatedEventState;
    }
} 