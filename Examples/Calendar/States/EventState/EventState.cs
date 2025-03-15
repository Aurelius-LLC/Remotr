
using System.Drawing;

namespace Remotr.Example.Calendar;

[GenerateSerializer]
public record EventState
{
    [Id(0)]
    public required Guid Id { get; set; }

    [Id(1)]
    public required string Title { get; set; }

    [Id(2)]
    public string? Description { get; set; }

    [Id(3)]
    public required DateOnly Date { get; set; }

    [Id(4)]
    public required TimeOnly StartTime { get; set; }

    [Id(5)]
    public required TimeOnly Duration { get; set; }

    [Id(6)]
    public string Color { get; set; } = "#000000";

    [Id(7)]
    public Dictionary<string, string> Tags { get; set; } = [];

    [Id(8)]
    public bool IsRecurring { get; set; }

    [Id(9)]
    public RecurrenceRule? RecurrenceRule { get; set; }
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