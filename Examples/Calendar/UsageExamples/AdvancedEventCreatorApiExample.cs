

namespace Remotr.Example.Calendar;

/// <summary>
/// Demonstrates a dynamic use of the command builders for an example API.
/// 
/// This is a more advanced example that shows how to use the command builders for an example API.
/// It is not recommended to use this pattern in a real application, but it is useful for demonstrating the capabilities of the command builders.
/// You could use this pattern for a REST API that allows users to dynamically create entities, but delays validation and persistence until the entity is saved, allowing for partially complete entities to be persisted in the form of persisting the builders.
/// TODO: Reference pack command above.
/// </summary>
public class AdvancedEventCreatorApiExample(IExternalCommandFactory commandFactory, IExternalQueryFactory queryFactory)
{
    private readonly IExternalCommandFactory commandFactory = commandFactory;

    private Guid? _eventId;
    private IGrainCommandBuilder<ICalendarManagerGrain, BaseStatelessCommandHandler<ICalendarManagerGrain>, BaseStatelessQueryHandler<ICalendarManagerGrain>, EventState>? _eventBuilder;

    /// <summary>
    /// Initializes the event builder with a new event state.
    /// Doesn't validate the event state or persist it yet.
    /// </summary>
    public void InitEvent()
    {
        // Initialize the event builder with a new event state.
        var _eventId = Guid.NewGuid();
        _eventBuilder = commandFactory.GetManager<ICalendarManagerGrain>()
            .AddEvent(new EventState {
                Id = _eventId
            });
    }

    /// <summary>
    /// Designating an event to be edited.
    /// Must validate the event exists.
    /// </summary>
    public async Task EditEvent(Guid eventId)
    {

        // Loading the event state in to the builder.
        _eventBuilder = commandFactory.GetManager<ICalendarManagerGrain>()
            .GetEvent(eventId);

        // Validate the event exists.
        var eventState = await _eventBuilder!.Run(eventId.ToString());
        if (eventState == null || eventState.Title == null || eventState.Title == string.Empty)
            throw new Exception("Event not found");

        _eventId = eventId;
    }

    /// <summary>
    /// Saving the event state.
    /// </summary>
    public async Task<EventState> Save()
    {
        return await _eventBuilder!.Run(_eventId!.Value.ToString());
    }



    // Methods allowing for dynamic creation of the event.
    
    public void UpdateTitle(string title)
    {
        _eventBuilder = _eventBuilder.UpdateEventTitle((_eventId!.Value, title));
    }

    public void UpdateDescription(string description)
    {
        _eventBuilder = _eventBuilder.UpdateEventDescription((_eventId!.Value, description));
    }

    public void UpdateStartTime(TimeOnly startTime)
    {
        _eventBuilder = _eventBuilder.UpdateEventTime((_eventId!.Value, startTime));
    }

    public void UpdateDuration(TimeOnly duration)
    {
        _eventBuilder = _eventBuilder.UpdateEventDuration((_eventId!.Value, duration));
    }

    public void UpdateColor(string color)
    {
        _eventBuilder = _eventBuilder.UpdateEventColor((_eventId!.Value, color));
    }

    public void UpdateRecurrenceRule(RecurrenceRule recurrenceRule)
    {
        _eventBuilder = _eventBuilder.UpdateEventRecurrenceRule((_eventId!.Value, recurrenceRule));
    }

    public void UpdateRecurring(bool recurring)
    {
        _eventBuilder = _eventBuilder.UpdateEventRecurring((_eventId!.Value, recurring));
    }
}
