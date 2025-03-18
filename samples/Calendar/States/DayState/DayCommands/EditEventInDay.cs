// TODO:
// Stateful command that updates an existing EventState in the DayState
// This command should be called by all stateless update commands (UpdateEventTitle, UpdateEventTime, etc.)
// It should update the event in the OrderedEvents list, maintaining the order by StartTime if StartTime was changed
// It should validate that the event exists in this DayState
// It should handle all potential side effects from any property changes:
//   - If StartTime changes: reorder the OrderedEvents list
//   - If Color, Title, Description, Duration, Tags, IsRecurring, or RecurrenceRule change: just update the event
// It should return the updated DayState 

namespace Remotr.Samples.Calendar;

[RemotrGen]
public class EditEventInDay : EntityCommandHandler<DayState, EventState, DayState>
{
    public override async Task<DayState> Execute(EventState updatedEvent)
    {
        var currentState = await GetState();
        
        // Find the existing event
        var existingEventIndex = currentState.OrderedEvents.FindIndex(e => e.Id == updatedEvent.Id);
        if (existingEventIndex == -1)
        {
            throw new ArgumentException($"Event with ID {updatedEvent.Id} not found in this DayState");
        }

        // Remove from current position
        currentState.OrderedEvents.RemoveAt(existingEventIndex);

        // Insert at new position if StartTime changed
        var insertIndex = currentState.OrderedEvents.FindIndex(e => e.StartTime > updatedEvent.StartTime);
        if (insertIndex == -1)
        {
            currentState.OrderedEvents.Add(updatedEvent);
        }
        else
        {
            currentState.OrderedEvents.Insert(insertIndex, updatedEvent);
        }

        await UpdateState(currentState);
        return currentState;
    }
} 