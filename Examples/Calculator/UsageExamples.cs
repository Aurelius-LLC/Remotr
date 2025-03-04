
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

public static class ExtensionsStatelessCommand1Type {
    public static IGrainCommandBaseBuilder<ICalculatorManagerGrain, BaseStatelessCommandHandler<ICalculatorManagerGrain>, BaseStatelessQueryHandler<ICalculatorManagerGrain>> SetValue1Type(this IGrainCommandBaseBuilder<ICalculatorManagerGrain, BaseStatelessCommandHandler<ICalculatorManagerGrain>, BaseStatelessQueryHandler<ICalculatorManagerGrain>> builder) 
    {
        return builder.Tell<SetValue1Type>();
    }

    public static IGrainCommandBaseBuilder<ICalculatorManagerGrain, BaseStatelessCommandHandler<ICalculatorManagerGrain>, BaseStatelessQueryHandler<ICalculatorManagerGrain>> SetValue1Type<T>(this IGrainCommandBuilder<ICalculatorManagerGrain, BaseStatelessCommandHandler<ICalculatorManagerGrain>, BaseStatelessQueryHandler<ICalculatorManagerGrain>, T> builder) {
        return builder.Tell<SetValue1Type>();
    }
}

public static class ExtensionsStatelessCommand2Type {
    public static IGrainCommandBuilder<ICalculatorManagerGrain, BaseStatelessCommandHandler<ICalculatorManagerGrain>, BaseStatelessQueryHandler<ICalculatorManagerGrain>, double> SetValue2Type(this IGrainCommandBaseBuilder<ICalculatorManagerGrain, BaseStatelessCommandHandler<ICalculatorManagerGrain>, BaseStatelessQueryHandler<ICalculatorManagerGrain>> builder, double input) 
    {
        return builder.Tell<SetValue2Type, double>();
    }

    public static IGrainCommandBuilder<ICalculatorManagerGrain, BaseStatelessCommandHandler<ICalculatorManagerGrain>, BaseStatelessQueryHandler<ICalculatorManagerGrain>, double> SetValue2Type<T>(this IGrainCommandBuilder<ICalculatorManagerGrain, BaseStatelessCommandHandler<ICalculatorManagerGrain>, BaseStatelessQueryHandler<ICalculatorManagerGrain>, T> builder, double input) {
        return builder.Tell<SetValue2Type, double>();
    }
}

public static class ExtensionsStatelessCommand3Types
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


public static class ExtensionsStatelessQuery1Types
{
    public static IGrainQueryBuilder<ICalculatorManagerGrain, BaseStatelessQueryHandler<ICalculatorManagerGrain>, double> GetValue(this IGrainQueryBaseBuilder<ICalculatorManagerGrain, BaseStatelessQueryHandler<ICalculatorManagerGrain>> builder) 
    {
        return builder.Ask<GetValue1Type, double>();
    }

    public static IGrainQueryBuilder<ICalculatorManagerGrain, BaseStatelessQueryHandler<ICalculatorManagerGrain>, double> GetValue<T>(this IGrainQueryBuilder<ICalculatorManagerGrain, BaseStatelessQueryHandler<ICalculatorManagerGrain>, T> builder, int input) 
    {
        return builder.Ask<GetValue1Type, double>();
    }
}



public static class ExtensionsStatelessQuery2Types
{
    public static IGrainQueryBuilder<ICalculatorManagerGrain, BaseStatelessQueryHandler<ICalculatorManagerGrain>, double> GetValue<T>(this IGrainQueryBuilder<ICalculatorManagerGrain, BaseStatelessQueryHandler<ICalculatorManagerGrain>, T> builder, int input) 
    {
        return builder.Ask<GetValue2Type, int, double>(input);
    }

    public static IGrainQueryBuilder<ICalculatorManagerGrain, BaseStatelessQueryHandler<ICalculatorManagerGrain>, double> ThenGetValue(this IGrainQueryBuilder<ICalculatorManagerGrain, BaseStatelessQueryHandler<ICalculatorManagerGrain>, int> builder) {
        return builder.ThenAsk<GetValue2Type, double>();
    }
}