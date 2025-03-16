namespace Remotr.Example.Calendar;

[RemotrGen]
public class CreateEventState : StatefulCommandHandler<EventState, EventState, EventState>
{
    public override async Task<EventState> Execute(EventState newState)
    {
        // Validate required fields
        if (string.IsNullOrEmpty(newState.Title))
        {
            throw new ArgumentException("Title cannot be null or empty");
        }
        
        // Ensure the ID is set
        if (newState.Id == Guid.Empty)
        {
            newState = newState with { Id = Guid.NewGuid() };
        }
        
        // Update the state with the new event
        await UpdateState(newState);
        
        return await GetState();
    }
}
