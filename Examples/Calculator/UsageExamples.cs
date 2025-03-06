
namespace Remotr.Example.Calculator;

public class Example1(IExternalCommandFactory factory)
{
    private readonly IExternalCommandFactory factory = factory;

    public async Task Multiply() {

        var x = factory.GetManager<ICalculatorManagerGrain>()
            .SetValue1Type()
            .Divide(2.0)
            .GetValue3Type(1)
            .ForEach(
                [2.0, 4.0],
                (builder) => builder.Multiply(2.0)
            )
            .SetValue3Type(30)
            .MergeSplit(
                (builder) => builder.ForEach(
                    [1, 2],
                    (builder) => builder.ThenSetValue3Type()
                ),
                (builder) => builder.SetValue3Type(21),
                new TakeSecond<IEnumerable<double>, double>()
            );
        
        x = x.SetValue3Type(20);
        
        await factory.GetManager<ICalculatorManagerGrain>()
            .SetValue3Type(30)
            .ForEach(
                [1, 2],
                (builder) => builder.GetValue2Type()
            )
            .Multiply(5.0)
            .Run("a");
    }
}