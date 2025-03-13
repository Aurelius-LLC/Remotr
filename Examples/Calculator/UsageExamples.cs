using Orleans.Runtime.GrainDirectory;

namespace Remotr.Example.Calculator;

public class Example1(IExternalCommandFactory factory)
{
    private readonly IExternalCommandFactory factory = factory;

    public async Task Multiply() {

        IGrainCommandBuilder<Remotr.Example.Calculator.ICalculatorManagerGrain, BaseStatelessCommandHandler<Remotr.Example.Calculator.ICalculatorManagerGrain>, BaseStatelessQueryHandler<Remotr.Example.Calculator.ICalculatorManagerGrain>, double> x = factory.GetManager<ICalculatorManagerGrain>()
            
            // This
            .Tell<Divide, double, double>(3)
            .Tell<Divide2, double, double>(3)

            // is functionally equivalent to
            .Divide(2.0)
            
            .SetValue1Type()
            .Divide(2.0)
            .GetValue3Type(2)
            .ForEach(
                [1, 2],
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
                (builder) => builder.GetValue3Type(2)
            )
            .Multiply(5.0)
            .Run("a");
    }
}