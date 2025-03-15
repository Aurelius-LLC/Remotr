// TODO:
// Stateful command that adds an EventState to the DayState
// This command should be called by the stateless AddEvent and RescheduleEvent commands
// It should add the event to the OrderedEvents list, maintaining the order by StartTime
// It should update the TimesToEventsWithReminders dictionary if the event has reminders
// It should validate that the event's Date matches this DayState's date
// It should return the updated DayState 