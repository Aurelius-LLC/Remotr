
namespace Remotr.Example.Calendar;

[GenerateSerializer]
public record EventState
{
    [Id(0)]
    public Guid Id { get; set; }

    [Id(1)]
    public string Title { get; set; }

    [Id(2)]
    public string? Description { get; set; }

    [Id(3)]
    public DateOnly Date { get; set; }

    [Id(4)]
    public TimeOnly StartTime { get; set; }

    [Id(5)]
    public TimeOnly Duration { get; set; }

    [Id(6)]
    public string Color { get; set; } = "#000000";

    [Id(7)]
    public bool IsRecurring { get; set; }

    [Id(8)]
    public RecurrenceRule? RecurrenceRule { get; set; }

    [Id(9)]
    public Dictionary<TimeSpan, ScheduledReminder>? Reminders { get; set; }
}

[GenerateSerializer]
public record RecurrenceRule
{
    [Id(0)]
    public required string Frequency { get; set; }

    [Id(1)]
    public required int Interval { get; set; }

    [Id(2)]
    public required DateOnly? Until { get; set; }
}

[GenerateSerializer]
public class ScheduledReminder
{
    [Id(0)]
    public bool Sent { get; set; }

    [Id(1)]
    public int Priority { get; set; }
}