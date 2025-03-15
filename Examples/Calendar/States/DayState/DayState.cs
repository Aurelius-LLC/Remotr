
namespace Remotr.Example.Calendar;


[GenerateSerializer]
public record DayState
{
    // Day child grains are identified by the date.
    [Id(0)]
    public required DateOnly Date { get; set; }

    [Id(1)]
    public List<EventState> OrderedEvents { get; set; } = [];

    [Id(2)]
    public Dictionary<TimeOnly, List<Guid>> TimesToEventsWithReminders { get; set; } = [];
}
