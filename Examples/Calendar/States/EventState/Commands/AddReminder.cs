// TODO:
// Stateful command that adds a reminder to the Reminders dictionary of the EventState
// This command should be called by the stateless AddEventReminder command
// It should validate that the TimeSpan is positive and not already in the dictionary
// It should initialize the Reminders dictionary if it's null
// It should add the reminder to the dictionary and return the updated state 