
namespace Remotr.Example.Calculator;

public class Example1(IExternalCommandFactory factory)
{
    private readonly IExternalCommandFactory factory = factory;

    public async Task Multiply() {

        var x = factory.GetManager<ICalculatorManagerGrain>()
            .SetValue(30.0)
            .Tell<SetValue, double, double>(20.0)
            .MergeSplit(
                (builder) => builder.ForEach(
                    [1.0, 2.0],
                    (builder) => builder.ThenSetValue()
                ),
                (builder) => builder.ThenSetValue(),
                new TakeSecond<IEnumerable<double>, double>()
            );
        
        x = x.SetValue(20);
        
        await factory.GetManager<ICalculatorManagerGrain>()
            .SetValue(30.0)
            .ForEach(
                [1.0, 2.0],
                (builder) => builder.ThenSetValue()
            )
            .Tell<Multiply, double, double>(5.0)
            .Run("a");
    }
}

public static class ExtensionsStateless1Type {
    public static IGrainCommandBaseBuilder<ICalculatorManagerGrain, BaseStatelessCommandHandler<ICalculatorManagerGrain>, BaseStatelessQueryHandler<ICalculatorManagerGrain>> SetValue1Type(this IGrainCommandBaseBuilder<ICalculatorManagerGrain, BaseStatelessCommandHandler<ICalculatorManagerGrain>, BaseStatelessQueryHandler<ICalculatorManagerGrain>> builder) 
    {
        return builder.Tell<SetValue1Type>();
    }

    public static IGrainCommandBaseBuilder<ICalculatorManagerGrain, BaseStatelessCommandHandler<ICalculatorManagerGrain>, BaseStatelessQueryHandler<ICalculatorManagerGrain>> SetValue1Type<T>(this IGrainCommandBuilder<ICalculatorManagerGrain, BaseStatelessCommandHandler<ICalculatorManagerGrain>, BaseStatelessQueryHandler<ICalculatorManagerGrain>, T> builder) {
        return builder.Tell<SetValue1Type>();
    }
}

public static class ExtensionsStateless2Type {
    public static IGrainCommandBuilder<ICalculatorManagerGrain, BaseStatelessCommandHandler<ICalculatorManagerGrain>, BaseStatelessQueryHandler<ICalculatorManagerGrain>, double> SetValue2Type(this IGrainCommandBaseBuilder<ICalculatorManagerGrain, BaseStatelessCommandHandler<ICalculatorManagerGrain>, BaseStatelessQueryHandler<ICalculatorManagerGrain>> builder, double input) 
    {
        return builder.Tell<SetValue2Type, double>();
    }

    public static IGrainCommandBuilder<ICalculatorManagerGrain, BaseStatelessCommandHandler<ICalculatorManagerGrain>, BaseStatelessQueryHandler<ICalculatorManagerGrain>, double> SetValue2Type<T>(this IGrainCommandBuilder<ICalculatorManagerGrain, BaseStatelessCommandHandler<ICalculatorManagerGrain>, BaseStatelessQueryHandler<ICalculatorManagerGrain>, T> builder, double input) {
        return builder.Tell<SetValue2Type, double>();
    }
}

public static class ExtensionsStateless3Types
{
    public static IGrainCommandBuilder<ICalculatorManagerGrain, BaseStatelessCommandHandler<ICalculatorManagerGrain>, BaseStatelessQueryHandler<ICalculatorManagerGrain>, double> SetValue(this IGrainCommandBaseBuilder<ICalculatorManagerGrain, BaseStatelessCommandHandler<ICalculatorManagerGrain>, BaseStatelessQueryHandler<ICalculatorManagerGrain>> builder, double input) 
    {
        return builder.Tell<SetValue, double, double>(input);
    }

    public static IGrainCommandBuilder<ICalculatorManagerGrain, BaseStatelessCommandHandler<ICalculatorManagerGrain>, BaseStatelessQueryHandler<ICalculatorManagerGrain>, double> SetValue<T>(this IGrainCommandBuilder<ICalculatorManagerGrain, BaseStatelessCommandHandler<ICalculatorManagerGrain>, BaseStatelessQueryHandler<ICalculatorManagerGrain>, T> builder, double input) {
        return builder.Tell<SetValue, double, double>(input);
    }

    public static IGrainCommandBuilder<ICalculatorManagerGrain, BaseStatelessCommandHandler<ICalculatorManagerGrain>, BaseStatelessQueryHandler<ICalculatorManagerGrain>, double> ThenSetValue(this IGrainCommandBuilder<ICalculatorManagerGrain, BaseStatelessCommandHandler<ICalculatorManagerGrain>, BaseStatelessQueryHandler<ICalculatorManagerGrain>, double> builder) {
        return builder.ThenTell<SetValue, double>();
    }
}