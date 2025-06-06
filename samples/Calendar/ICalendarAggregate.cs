using Remotr;

namespace Remotr.Samples.Calendar;

// Example of using the fixedKey attribute to specify the key for the state.
// This is useful for having a singleton pattern for a particular state within a grain partition.
[UseCommand(typeof(CalendarAddEvent), "AddEvent", fixedKey: CalendarManagerKey)]
[UseCommand(typeof(CalendarAddEventReminder), "AddEventReminder", fixedKey: CalendarManagerKey)]
[UseCommand(typeof(CalendarRemoveEventReminder), "RemoveEventReminder", fixedKey: CalendarManagerKey)]
[UseCommand(typeof(CalendarRescheduleEvent), "RescheduleEvent", fixedKey: CalendarManagerKey)]
[UseCommand(typeof(CalendarUpdateEventColor), "UpdateEventColor", fixedKey: CalendarManagerKey)]
[UseCommand(typeof(CalendarUpdateEventDescription), "UpdateEventDescription", fixedKey: CalendarManagerKey)]
[UseCommand(typeof(CalendarUpdateEventDuration), "UpdateEventDuration", fixedKey: CalendarManagerKey)]
[UseCommand(typeof(CalendarUpdateEventRecurrenceRule), "UpdateEventRecurrenceRule", fixedKey: CalendarManagerKey)]
[UseCommand(typeof(CalendarUpdateEventRecurring), "UpdateEventRecurring", fixedKey: CalendarManagerKey)]
[UseCommand(typeof(CalendarUpdateEventTime), "UpdateEventTime", fixedKey: CalendarManagerKey)]
[UseCommand(typeof(CalendarUpdateEventTitle), "UpdateEventTitle", fixedKey: CalendarManagerKey)]
[UseCommand(typeof(CalendarFailCommand), "Fail", fixedKey: CalendarManagerKey)]
[UseCommand(typeof(CalendarUpdateEventStateInput), "UpdateEventStateInput", fixedKey: CalendarManagerKey)]
[UseQuery(typeof(CalendarGetEventsOnDay), "GetDaysEvents", fixedKey: CalendarManagerKey)]
[UseQuery(typeof(CalendarGetEvent), "GetEvent", fixedKey: CalendarManagerKey)]
public interface ICalendarAggregate : IAggregateRoot, IGrainWithStringKey
{
    const string CalendarManagerKey = "CalendarManagerState";
}