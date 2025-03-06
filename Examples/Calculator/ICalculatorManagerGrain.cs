using Remotr;

namespace Remotr.Example.Calculator;

[CqrsCollection(typeof(DivideState), "Divide")]
[CqrsCollection(typeof(SetValueState1Type), "SetValue1Type")]
public interface ICalculatorManagerGrain : ITransactionManagerGrain, IGrainWithStringKey
{
}