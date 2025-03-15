// TODO:
// Stateless event that removes a reminder from a given event, based on its Id found from the command's input
// The reminder is identified by a TimeSpan representing minutes before the event
// While it should take an EventState as an input, this shouldn't be capable of creating an Event
// It must update the EventState of the EventState grain as well as the EventState in the DayState grain
// Should return the EventState. 