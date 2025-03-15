// TODO:
// Stateless event that updates a given event, based on its Id found from the command's input, with a new date
// This shouldn't be capable of creating an Event
// It must update the EventState of the EventState grain
// It must remove the event from the old DayState and add it to the new DayState (if it's on a different date)
// Should return the EventState. 