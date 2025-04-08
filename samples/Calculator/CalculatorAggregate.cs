using Microsoft.Extensions.DependencyInjection;

namespace Remotr.Samples.Calculator;

// DI persistent store into the Aggregate.
public class CalculatorAggregate([FromKeyedServices("Calculator")] IPersistentStore persistentStore) : AggregateRoot<ICalculatorAggregate>(persistentStore), ICalculatorAggregate
{
}