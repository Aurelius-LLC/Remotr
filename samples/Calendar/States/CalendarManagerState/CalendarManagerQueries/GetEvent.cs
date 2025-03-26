namespace Remotr.Samples.Calendar;

[UseShortcuts]
public class CalendarGetEvent : EntityQueryHandler<CalendarManagerState, Guid, EventState>
{
    public override async Task<EventState> Execute(Guid eventId)
    {
        // Get the event directly from the EventState child grain
        var eventState = await QueryFactory.GetEntity<EventState>()
            .GetEventState()
            .Run(eventId.ToString());
            
        return eventState;
    }
}
