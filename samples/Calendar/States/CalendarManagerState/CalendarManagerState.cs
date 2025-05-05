

namespace Remotr.Samples.Calendar;

[GenerateSerializer]
public record CalendarManagerState
{
    [Id(0)]
    public Dictionary<DateTime, List<Guid>> DateToEventsWithReminders { get; set; } = [];
}