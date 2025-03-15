
namespace Remotr.Example.Calendar;

[GenerateSerializer]
public record DayState
{
    [Id(0)]
    public required DateOnly Date { get; set; }

    [Id(1)]
    public List<EventState> OrderedEvents { get; set; } = [];
}
