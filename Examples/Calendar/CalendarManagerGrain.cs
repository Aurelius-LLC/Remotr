

using Remotr;

namespace Remotr.Example.Calendar;

public class CalendarManagerGrain([FromKeyedServices("Calendar")] IPersistentStore persistentStore) : TransactionManagerGrain<ICalendarManagerGrain>(persistentStore)
{
}