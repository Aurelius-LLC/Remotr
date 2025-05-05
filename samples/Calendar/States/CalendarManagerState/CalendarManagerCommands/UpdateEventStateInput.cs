
namespace Remotr.Samples.Calendar;

[UseShortcuts]
public class CalendarUpdateEventStateInput : EntityCommandHandler<CalendarManagerState, EventState, EventState>
{
    public override Task<EventState> Execute(EventState input)
    {
        input.Duration = new TimeOnly(ticks: 1);
        return Task.FromResult(input);
    }
}