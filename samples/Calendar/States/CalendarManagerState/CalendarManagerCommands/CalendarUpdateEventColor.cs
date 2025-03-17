namespace Remotr.Samples.Calendar;

[RemotrGen]
public class CalendarUpdateEventColor : StatefulCommandHandler<CalendarManagerState, (Guid eventId, string color), EventState>
{
    public override async Task<EventState> Execute((Guid eventId, string color) input)
    {
        // Get the event ID
        var eventId = input.eventId;
        
        // Update the event color
        var updatedEventState = await CommandFactory.GetChild<EventState>()
            .UpdateColor(input.color)
            .Run(eventId.ToString());
            
        // Update the event in the day state
        await CommandFactory.GetChild<DayState>()
            .EditEventInDay(updatedEventState)
            .Run(updatedEventState.Date.ToString());
            
        return updatedEventState;
    }
} 