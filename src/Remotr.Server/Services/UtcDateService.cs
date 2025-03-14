using System.Diagnostics;

namespace Remotr;

public class UtcDateService : IUtcDateService
{
    private static DateTime? DateOverrideSetTime { get; set; }
    private static DateTime? DateOverrideValue { get; set; }

    public static DateTime? DateOverride
    {
        get
        {
            var elapsedTime = UtcNowTruncatedToMilliseconds() - DateOverrideSetTime;
            return DateOverrideValue + elapsedTime;
        }
        set
        {
            DateOverrideSetTime = UtcNowTruncatedToMilliseconds();
            DateOverrideValue = DateTimeTruncatedToMilliseconds(value);
        }
    }

    public DateTime GetUtcDate()
    {
        if (DateOverride != null)
        {
            return DateOverride.Value;
        }
        return UtcNowTruncatedToMilliseconds();
    }

    private static DateTime UtcNowTruncatedToMilliseconds()
    {
        var result = DateTimeTruncatedToMilliseconds(DateTime.UtcNow);
        if (!result.HasValue)
        {
            throw new UnreachableException("Should have returned a non-null result");
        }
        return result.Value;
    }

    private static DateTime? DateTimeTruncatedToMilliseconds(DateTime? dateTime) =>
        dateTime is null ? null : new DateTime(dateTime.Value.Year, dateTime.Value.Month, dateTime.Value.Day, dateTime.Value.Hour, dateTime.Value.Minute, dateTime.Value.Second, dateTime.Value.Millisecond, dateTime.Value.Kind);
}
