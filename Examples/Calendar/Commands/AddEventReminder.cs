// TODO:
// Stateless event that adds a reminder to a given event, based on its Id
// The reminder is identified by a TimeSpan representing minutes before the event
// This shouldn't be capable of creating an Event
// It must update the EventState of the EventState grain as well as the EventState in the DayState grain
// Should return the EventState. 