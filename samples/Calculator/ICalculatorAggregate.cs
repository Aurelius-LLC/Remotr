namespace Remotr.Samples.Calculator;


[UseCommand(typeof(MultiplyState), "Multiply")]
[UseCommand(typeof(DivideState), "Divide")]
[UseCommand(typeof(SetState), "Set")]
[UseCommand(typeof(AddState), "Add")]
[UseQuery(typeof(ArtificialDelay), "Delay")]
[UseQuery(typeof(GetValueState<CalculatorState>), "GetValue")]
[UseQuery(typeof(GetPrimeFactorsState), "GetPrimeFactors")]
public interface ICalculatorAggregate : IAggregateRoot, IGrainWithStringKey
{
}