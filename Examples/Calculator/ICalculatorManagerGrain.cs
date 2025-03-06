using Remotr;

namespace Remotr.Example.Calculator;

[Alias("CalculatorManager")]
[CqrsCollection(
    typeof(SetValueState1Type), 
    "SetValue1Type"
)]
[CqrsCollection(typeof(SetValueState2Type), "SetValue2Type")]
[CqrsCollection(typeof(SetValueState3Type), "SetValue3Type")]
[CqrsCollection(typeof(MultiplyState), "Multiply")]
[CqrsCollection(typeof(DivideState), "Divide")]
[CqrsCollection(typeof(GetPrimeFactorsState), "GetPrimeFactors")]
[CqrsCollection(typeof(GetValueState3Type), "GetValue3Type")]
public interface ICalculatorManagerGrain : ITransactionManagerGrain, IGrainWithStringKey
{
    public static int MethodTest() {
        return 1;
    }
}