// TODO:
// Stateful command that removes a reminder from the Reminders dictionary of the EventState
// This command should be called by the stateless RemoveEventReminder command
// It should validate that the Reminders dictionary is not null and contains the specified TimeSpan
// It should remove the reminder from the dictionary and return the updated state 