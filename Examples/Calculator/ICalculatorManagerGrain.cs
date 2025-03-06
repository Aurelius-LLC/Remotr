using Remotr;

namespace Remotr.Example.Calculator;

[CqrsCollection(typeof(DivideState), "Divide")]
[CqrsCollection(typeof(SetValueState1Type), "SetValue1Type")]
[CqrsCollection(typeof(GetPrimeFactorsState), "GetPrimeFactors")]
[CqrsCollection(typeof(GetValueState3Type), "GetValue3Type")]
public interface ICalculatorManagerGrain : ITransactionManagerGrain, IGrainWithStringKey
{
}