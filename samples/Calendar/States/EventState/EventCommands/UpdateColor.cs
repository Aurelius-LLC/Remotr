namespace Remotr.Samples.Calendar;

[RemotrGen]
public class UpdateColor : StatefulCommandHandler<EventState, string, EventState>
{
    public override async Task<EventState> Execute(string color)
    {
        if (string.IsNullOrEmpty(color))
        {
            throw new ArgumentException("Color cannot be null or empty");
        }

        var currentState = await GetState();
        
        // Create a new state with the updated color
        var updatedState = currentState with { Color = color };
        
        await UpdateState(updatedState);
        
        return await GetState();
    }
} 