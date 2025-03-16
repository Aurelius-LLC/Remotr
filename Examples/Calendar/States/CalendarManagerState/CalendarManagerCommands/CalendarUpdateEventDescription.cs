namespace Remotr.Example.Calendar;

[RemotrGen]
public class CalendarUpdateEventDescription : StatefulCommandHandler<CalendarManagerState, (Guid eventId, string description), EventState>
{
    public override async Task<EventState> Execute((Guid eventId, string description) input)
    {
        // Get the event ID
        var eventId = input.eventId;
        
        // Update the event description
        var updatedEventState = await CommandFactory.GetChild<EventState>()
            .UpdateDescription(input.description)
            .Run(eventId.ToString());
            
        // Update the event in the day state
        await CommandFactory.GetChild<DayState>()
            .EditEventInDay(updatedEventState)
            .Run(updatedEventState.Date.ToString());
            
        return updatedEventState;
    }
} 