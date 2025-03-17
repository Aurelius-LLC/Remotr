using Microsoft.Extensions.DependencyInjection;

namespace Remotr.Samples.Calculator;

// DI persistent store into the ManagerGrain.
public class CalculatorManagerGrain([FromKeyedServices("Calculator")] IPersistentStore persistentStore) : TransactionManagerGrain<ICalculatorManagerGrain>(persistentStore)
{
}