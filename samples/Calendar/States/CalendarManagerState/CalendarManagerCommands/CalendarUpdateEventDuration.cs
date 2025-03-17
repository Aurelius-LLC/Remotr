namespace Remotr.Samples.Calendar;

[RemotrGen]
public class CalendarUpdateEventDuration : StatefulCommandHandler<CalendarManagerState, (Guid eventId, TimeOnly duration), EventState>
{
    public override async Task<EventState> Execute((Guid eventId, TimeOnly duration) input)
    {
        // Get the event ID
        var eventId = input.eventId;
        
        // Update the event duration
        var updatedEventState = await CommandFactory.GetChild<EventState>()
            .UpdateDuration(input.duration)
            .Run(eventId.ToString());
            
        // Update the event in the day state
        await CommandFactory.GetChild<DayState>()
            .EditEventInDay(updatedEventState)
            .Run(updatedEventState.Date.ToString());
            
        return updatedEventState;
    }
} 