// TODO:
// Stateful command that updates the Title property of the EventState
// This command should be called by the stateless UpdateEventTitle command
// It should validate that the title is not null or empty
// It should update the EventState and return the updated state 

namespace Remotr.Samples.Calendar;

[UseShortcuts]
public class UpdateTitle : EntityCommandHandler<EventState, string, EventState>
{
    public override async Task<EventState> Execute(string title)
    {
        if (string.IsNullOrEmpty(title))
        {
            throw new ArgumentException("Title cannot be null or empty");
        }

        var currentState = await GetState();
        
        // Create a new state with the updated title
        var updatedState = currentState with { Title = title };
        
        await UpdateState(updatedState);
        
        return await GetState();
    }
} 