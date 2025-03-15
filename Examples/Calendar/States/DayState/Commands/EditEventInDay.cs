// TODO:
// Stateful command that updates an existing EventState in the DayState
// This command should be called by all stateless update commands (UpdateEventTitle, UpdateEventTime, etc.)
// It should update the event in the OrderedEvents list, maintaining the order by StartTime if StartTime was changed
// It should update the TimesToEventsWithReminders dictionary if reminders were added, removed, or modified
// It should validate that the event exists in this DayState
// It should handle all potential side effects from any property changes:
//   - If StartTime changes: reorder the OrderedEvents list
//   - If Reminders change: update the TimesToEventsWithReminders dictionary
//   - If Color, Title, Description, Duration, Tags, IsRecurring, or RecurrenceRule change: just update the event
// It should return the updated DayState 