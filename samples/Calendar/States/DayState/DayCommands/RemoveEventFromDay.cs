namespace Remotr.Samples.Calendar;

[RemotrGen]
public class RemoveEventFromDay : EntityCommandHandler<DayState, Guid, DayState>
{
    public override async Task<DayState> Execute(Guid eventId)
    {
        var currentState = await GetState();
        
        // Find and validate the event exists
        var existingEventIndex = currentState.OrderedEvents.FindIndex(e => e.Id == eventId);
        if (existingEventIndex == -1)
        {
            throw new ArgumentException($"Event with ID {eventId} not found in this DayState");
        }

        // Remove from OrderedEvents
        currentState.OrderedEvents.RemoveAt(existingEventIndex);

        await UpdateState(currentState);
        return currentState;
    }
} 