namespace Remotr.Samples.Calendar;

[UseShortcuts]
public class AddEventToDay : EntityCommandHandler<DayState, EventState, DayState>
{
    public override async Task<DayState> Execute(EventState input)
    {
        var currentState = await GetState();
        
        // Validate event date matches DayState date
        if (input.Date != currentState.Date)
        {
            throw new ArgumentException($"Event date {input.Date} does not match DayState date {currentState.Date}");
        }

        // Add event to OrderedEvents, maintaining order by StartTime
        var events = currentState.OrderedEvents;
        var insertIndex = events.FindIndex(e => e.StartTime > input.StartTime);
        if (insertIndex == -1)
        {
            events.Add(input);
        }
        else
        {
            events.Insert(insertIndex, input);
        }

        await UpdateState(currentState);
        return currentState;
    }
} 