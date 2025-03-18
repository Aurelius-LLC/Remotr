namespace Remotr.Samples.Calendar;

[RemotrGen]
public class GetEventState : EntityQueryHandler<EventState, EventState>
{
    public override async Task<EventState> Execute()
    {
        // Simply return the current state of the event
        return await GetState();
    }
}
