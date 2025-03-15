// TODO:
// Stateless event that updates a given event, based on its Id, with a new recurrence rule from the command's input
// This shouldn't be capable of creating an Event
// It must update the EventState of the EventState grain as well as the EventState in the DayState grain
// Should return the EventState. 