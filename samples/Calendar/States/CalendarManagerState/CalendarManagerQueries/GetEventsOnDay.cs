namespace Remotr.Samples.Calendar;

[UseShortcuts]
public class CalendarGetEventsOnDay : EntityQueryHandler<CalendarManagerState, DateOnly, List<EventState>>
{
    public override async Task<List<EventState>> Execute(DateOnly date)
    {
        // Get all events for the specified day
        var dayEvents = await QueryFactory.GetEntity<DayState>()
            .GetDayStateEvents()
            .Run(date.ToString());
            
        return dayEvents;
    }
} 