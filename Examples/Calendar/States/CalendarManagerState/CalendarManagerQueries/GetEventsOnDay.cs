namespace Remotr.Example.Calendar;

[RemotrGen]
public class CalendarGetEventsOnDay : StatefulQueryHandler<CalendarManagerState, DateOnly, List<EventState>>
{
    public override async Task<List<EventState>> Execute(DateOnly date)
    {
        // Get all events for the specified day
        var dayEvents = await QueryFactory.GetChild<DayState>()
            .GetDayStateEvents()
            .Run(date.ToString());
            
        return dayEvents;
    }
} 