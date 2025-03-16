namespace Remotr.Example.Calendar;

[RemotrGen]
public class GetEventState : StatefulQueryHandler<EventState, EventState>
{
    public override async Task<EventState> Execute()
    {
        // Simply return the current state of the event
        return await GetState();
    }
}
