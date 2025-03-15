using Microsoft.Extensions.DependencyInjection;

namespace Remotr.Example.Calculator;

// DI persistent store into the ManagerGrain.
public class CalculatorManagerGrain([FromKeyedServices("Calculator")] IPersistentStore persistentStore) : TransactionManagerGrain<ICalculatorManagerGrain>(persistentStore)
{
}