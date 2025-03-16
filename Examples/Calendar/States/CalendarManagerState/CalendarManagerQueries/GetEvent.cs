namespace Remotr.Example.Calendar;

[RemotrGen]
public class CalendarGetEvent : StatefulQueryHandler<CalendarManagerState, Guid, EventState>
{
    public override async Task<EventState> Execute(Guid eventId)
    {
        // Get the event directly from the EventState child grain
        var eventState = await QueryFactory.GetChild<EventState>()
            .GetEventState()
            .Run(eventId.ToString());
            
        return eventState;
    }
}
