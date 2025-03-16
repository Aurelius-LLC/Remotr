namespace Remotr.Example.Calendar;


[GenerateSerializer]
public record DayState
{
    // Day child grains are identified by the date.
    [Id(0)]
    public DateOnly Date { get; set; }

    [Id(1)]
    public List<EventState> OrderedEvents { get; set; } = [];
}
