

namespace Remotr.Samples.Calendar;

public class CalendarFailCommand : EntityCommandHandler<CalendarManagerState>
{
    public override Task Execute()
    {
        throw new Exception("This command is designed to fail!");
    }
}

