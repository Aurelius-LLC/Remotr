using Remotr;

namespace Remotr.Example.Calculator;


[UseCommand(typeof(SetValueState1Type), "SetValue1Type")]
[UseCommand(typeof(SetValueState2Type), "SetValue2Type")]
[UseCommand(typeof(SetValueState3Type), "SetValue3Type")]
[UseCommand(typeof(MultiplyState), "Multiply")]
[UseCommand(typeof(DivideState), "Divide")]
[UseQuery(typeof(GetPrimeFactorsState), "GetPrimeFactors")]
[UseQuery(typeof(GetValueState3Type), "GetValue3Type")]
public interface ICalculatorManagerGrain : ITransactionManagerGrain, IGrainWithStringKey
{
    public static int MethodTest() {
        return 1;
    }
}