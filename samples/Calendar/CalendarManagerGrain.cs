

using Remotr;

namespace Remotr.Samples.Calendar;

public class CalendarManagerGrain([FromKeyedServices("Calendar")] IPersistentStore persistentStore) : TransactionManagerGrain<ICalendarManagerGrain>(persistentStore)
{
}