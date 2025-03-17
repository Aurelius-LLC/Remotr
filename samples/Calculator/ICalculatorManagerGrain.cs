namespace Remotr.Samples.Calculator;


[UseCommand(typeof(MultiplyState), "Multiply")]
[UseCommand(typeof(DivideState), "Divide")]
[UseCommand(typeof(SetState), "Set")]
[UseCommand(typeof(AddState), "Add")]
[UseQuery(typeof(GetValueState<CalculatorState>), "GetValue")]
[UseQuery(typeof(GetPrimeFactorsState), "GetPrimeFactors")]
public interface ICalculatorManagerGrain : ITransactionManagerGrain, IGrainWithStringKey
{
}