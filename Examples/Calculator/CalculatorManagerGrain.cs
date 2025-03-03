using Microsoft.Extensions.DependencyInjection;

namespace Remotr.Example.Calculator;

public class CalculatorManagerGrain([FromKeyedServices("Calculator")] IPersistentStore persistentStore) : TransactionManagerGrain<ICalculatorManagerGrain>(persistentStore)
{
}