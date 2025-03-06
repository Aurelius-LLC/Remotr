using Remotr;

namespace Remotr.Example.Calculator;

// Example of using the CqrsCollection attribute to generate stateless commands/queries from stateful ones
[CqrsCollection(typeof(DivideState), "Divide")]
public interface ICalculatorManagerGrain : ITransactionManagerGrain, IGrainWithStringKey
{
}