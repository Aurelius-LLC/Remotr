using Remotr;

namespace Remotr.Example.Calendar;


public interface ICalendarManagerGrain : ITransactionManagerGrain, IGrainWithStringKey
{
}