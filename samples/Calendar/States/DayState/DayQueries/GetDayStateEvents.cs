namespace Remotr.Samples.Calendar;

[RemotrGen]
public class GetDayStateEvents : StatefulQueryHandler<DayState, List<EventState>>
{
    public override async Task<List<EventState>> Execute()
    {
        // Get the current day state
        var dayState = await GetState();
        
        // Return the ordered list of events for this day
        return dayState.OrderedEvents;
    }
}
