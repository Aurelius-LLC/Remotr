// TODO:
// Stateful command that removes an EventState from the DayState
// This command should be called by the stateless RescheduleEvent command when moving an event to a different day
// It should remove the event from the OrderedEvents list
// It should remove any entries for this event from the TimesToEventsWithReminders dictionary
// It should validate that the event exists in this DayState
// It should return the updated DayState 