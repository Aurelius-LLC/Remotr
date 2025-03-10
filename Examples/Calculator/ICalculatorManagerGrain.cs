namespace Remotr.Example.Calculator;


[UseCommand(typeof(SetValueState1Type), "SetValue1Type", findMethod: nameof(SetValue1TypeKey))]
[UseCommand(typeof(SetValueState2Type), "SetValue2Type")]
[UseCommand(typeof(SetValueState3Type), "SetValue3Type")]
[UseCommand(typeof(MultiplyState), "Multiply")]
[UseCommand(typeof(DivideState), "Divide", fixedKey: "fixed-divide-key")]
[UseQuery(typeof(GetPrimeFactorsState), "GetPrimeFactors")]
[UseQuery(typeof(GetValueState3Type), "GetValue3Type",  findMethod: nameof(GetValueState3TypeKey))]
public interface ICalculatorManagerGrain : ITransactionManagerGrain, IGrainWithStringKey
{
    public static int MethodTest() {
        return 1;
    }

    public static string SetValue1TypeKey() {
        return "Calculator";
    }

    public static string GetValueState3TypeKey(int input) {
        return "Calculator";
    }
}