

using Remotr;

namespace Remotr.Samples.Calendar;

public class CalendarAggregate([FromKeyedServices("Calendar")] IPersistentStore persistentStore) : AggregateRoot<ICalendarAggregate>(persistentStore)
{
}