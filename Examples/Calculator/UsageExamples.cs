
namespace Remotr.Example.Calculator;

public class Example1(IExternalCommandFactory factory)
{
    private readonly IExternalCommandFactory factory = factory;

    public async Task Multiply() {

        var x = factory.GetManager<ICalculatorManagerGrain>()
            .SetValue3Type(30)
            .Tell<SetValue3Type, int, double>(20)
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
                (builder) => builder.ThenSetValue3Type()
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
    public static IGrainCommandBuilder<ICalculatorManagerGrain, BaseStatelessCommandHandler<ICalculatorManagerGrain>, BaseStatelessQueryHandler<ICalculatorManagerGrain>, double> SetValue2Type(this IGrainCommandBaseBuilder<ICalculatorManagerGrain, BaseStatelessCommandHandler<ICalculatorManagerGrain>, BaseStatelessQueryHandler<ICalculatorManagerGrain>> builder) 
    {
        return builder.Tell<SetValue2Type, double>();
    }

    public static IGrainCommandBuilder<ICalculatorManagerGrain, BaseStatelessCommandHandler<ICalculatorManagerGrain>, BaseStatelessQueryHandler<ICalculatorManagerGrain>, double> SetValue2Type<T>(this IGrainCommandBuilder<ICalculatorManagerGrain, BaseStatelessCommandHandler<ICalculatorManagerGrain>, BaseStatelessQueryHandler<ICalculatorManagerGrain>, T> builder) {
        return builder.Tell<SetValue2Type, double>();
    }
}

public static class ExtensionsStatelessCommand3Types
{
    public static IGrainCommandBuilder<ICalculatorManagerGrain, BaseStatelessCommandHandler<ICalculatorManagerGrain>, BaseStatelessQueryHandler<ICalculatorManagerGrain>, double> SetValue3Type(this IGrainCommandBaseBuilder<ICalculatorManagerGrain, BaseStatelessCommandHandler<ICalculatorManagerGrain>, BaseStatelessQueryHandler<ICalculatorManagerGrain>> builder, int input) 
    {
        return builder.Tell<SetValue3Type, int, double>(input);
    }

    public static IGrainCommandBuilder<ICalculatorManagerGrain, BaseStatelessCommandHandler<ICalculatorManagerGrain>, BaseStatelessQueryHandler<ICalculatorManagerGrain>, double> SetValue3Type<T>(this IGrainCommandBuilder<ICalculatorManagerGrain, BaseStatelessCommandHandler<ICalculatorManagerGrain>, BaseStatelessQueryHandler<ICalculatorManagerGrain>, T> builder, int input) {
        return builder.Tell<SetValue3Type, int, double>(input);
    }

    public static IGrainCommandBuilder<ICalculatorManagerGrain, BaseStatelessCommandHandler<ICalculatorManagerGrain>, BaseStatelessQueryHandler<ICalculatorManagerGrain>, double> ThenSetValue3Type(this IGrainCommandBuilder<ICalculatorManagerGrain, BaseStatelessCommandHandler<ICalculatorManagerGrain>, BaseStatelessQueryHandler<ICalculatorManagerGrain>, int> builder) {
        return builder.ThenTell<SetValue3Type, double>();
    }
}


public static class ExtensionsStatelessQuery2Types
{
    public static IGrainQueryBuilder<ICalculatorManagerGrain, BaseStatelessQueryHandler<ICalculatorManagerGrain>, double> GetValue2Type(this IGrainQueryBaseBuilder<ICalculatorManagerGrain, BaseStatelessQueryHandler<ICalculatorManagerGrain>> builder) 
    {
        return builder.Ask<GetValue2Type, double>();
    }

    public static IGrainCommandBuilder<ICalculatorManagerGrain, BaseStatelessCommandHandler<ICalculatorManagerGrain>, BaseStatelessQueryHandler<ICalculatorManagerGrain>, double> GetValue2Type(this IGrainCommandBaseBuilder<ICalculatorManagerGrain, BaseStatelessCommandHandler<ICalculatorManagerGrain>, BaseStatelessQueryHandler<ICalculatorManagerGrain>> builder) 
    {
        return builder.Ask<GetValue2Type, double>();
    }

    public static IGrainQueryBuilder<ICalculatorManagerGrain, BaseStatelessQueryHandler<ICalculatorManagerGrain>, double> GetValue2Type<T>(this IGrainQueryBuilder<ICalculatorManagerGrain, BaseStatelessQueryHandler<ICalculatorManagerGrain>, T> builder, int input) 
    {
        return builder.Ask<GetValue2Type, double>();
    }

    public static IGrainCommandBuilder<ICalculatorManagerGrain, BaseStatelessCommandHandler<ICalculatorManagerGrain>, BaseStatelessQueryHandler<ICalculatorManagerGrain>, double> GetValue2Type<T>(this IGrainCommandBuilder<ICalculatorManagerGrain, BaseStatelessCommandHandler<ICalculatorManagerGrain>, BaseStatelessQueryHandler<ICalculatorManagerGrain>, T> builder) {
        return builder.Ask<GetValue2Type, double>();
    }
}



public static class ExtensionsStatelessQuery3Types
{
    public static IGrainQueryBuilder<ICalculatorManagerGrain, BaseStatelessQueryHandler<ICalculatorManagerGrain>, double> GetValue3Type(this IGrainQueryBaseBuilder<ICalculatorManagerGrain, BaseStatelessQueryHandler<ICalculatorManagerGrain>> builder, int input) 
    {
        return builder.Ask<GetValue3Type, int, double>(input);
    }
    
    public static IGrainQueryBuilder<ICalculatorManagerGrain, BaseStatelessQueryHandler<ICalculatorManagerGrain>, double> GetValue3Type<T>(this IGrainQueryBuilder<ICalculatorManagerGrain, BaseStatelessQueryHandler<ICalculatorManagerGrain>, T> builder, int input) 
    {
        return builder.Ask<GetValue3Type, int, double>(input);
    }

    public static IGrainCommandBuilder<ICalculatorManagerGrain, BaseStatelessCommandHandler<ICalculatorManagerGrain>, BaseStatelessQueryHandler<ICalculatorManagerGrain>, double> GetValue3Type(this IGrainCommandBaseBuilder<ICalculatorManagerGrain, BaseStatelessCommandHandler<ICalculatorManagerGrain>, BaseStatelessQueryHandler<ICalculatorManagerGrain>> builder, int input) 
    {
        return builder.Ask<GetValue3Type, int, double>(input);
    }

    public static IGrainCommandBuilder<ICalculatorManagerGrain, BaseStatelessCommandHandler<ICalculatorManagerGrain>, BaseStatelessQueryHandler<ICalculatorManagerGrain>, double> GetValue3Type<T>(this IGrainCommandBuilder<ICalculatorManagerGrain, BaseStatelessCommandHandler<ICalculatorManagerGrain>, BaseStatelessQueryHandler<ICalculatorManagerGrain>, T> builder, int input) 
    {
        return builder.Ask<GetValue3Type, int, double>(input);
    }

    public static IGrainQueryBuilder<ICalculatorManagerGrain, BaseStatelessQueryHandler<ICalculatorManagerGrain>, double> ThenGetValue3Type(this IGrainQueryBuilder<ICalculatorManagerGrain, BaseStatelessQueryHandler<ICalculatorManagerGrain>, int> builder) {
        return builder.ThenAsk<GetValue3Type, double>();
    }

    public static IGrainCommandBuilder<ICalculatorManagerGrain, BaseStatelessCommandHandler<ICalculatorManagerGrain>, BaseStatelessQueryHandler<ICalculatorManagerGrain>, double> ThenGetValue3Type(this IGrainCommandBuilder<ICalculatorManagerGrain, BaseStatelessCommandHandler<ICalculatorManagerGrain>, BaseStatelessQueryHandler<ICalculatorManagerGrain>, int> builder) 
    {
        return builder.ThenAsk<GetValue3Type, double>();
    }
}




public static class ExtensionsStatefulCommand1Types
{
    public static IGrainCommandBaseBuilder<ITransactionChildGrain<CalculatorState>, BaseStatefulCommandHandler<CalculatorState>, BaseStatefulQueryHandler<CalculatorState>> SetValueState1Type(this IGrainCommandBaseBuilder<ITransactionChildGrain<CalculatorState>, BaseStatefulCommandHandler<CalculatorState>, BaseStatefulQueryHandler<CalculatorState>> builder) 
    {
        return builder.Tell<SetValueState1Type>();
    }

    public static IGrainCommandBaseBuilder<ITransactionChildGrain<CalculatorState>, BaseStatefulCommandHandler<CalculatorState>, BaseStatefulQueryHandler<CalculatorState>> SetValueState1Type<T>(this IGrainCommandBuilder<ITransactionChildGrain<CalculatorState>, BaseStatefulCommandHandler<CalculatorState>, BaseStatefulQueryHandler<CalculatorState>, T> builder) {
        return builder.Tell<SetValueState1Type>();
    }
}


public static class ExtensionsStatefulCommand2Types
{
    public static IGrainCommandBuilder<ITransactionChildGrain<CalculatorState>, BaseStatefulCommandHandler<CalculatorState>, BaseStatefulQueryHandler<CalculatorState>, double> SetValueState2Type(this IGrainCommandBaseBuilder<ITransactionChildGrain<CalculatorState>, BaseStatefulCommandHandler<CalculatorState>, BaseStatefulQueryHandler<CalculatorState>> builder) 
    {
        return builder.Tell<SetValueState2Type, double>();
    }

    public static IGrainCommandBuilder<ITransactionChildGrain<CalculatorState>, BaseStatefulCommandHandler<CalculatorState>, BaseStatefulQueryHandler<CalculatorState>, double> SetValueState2Type<T>(this IGrainCommandBuilder<ITransactionChildGrain<CalculatorState>, BaseStatefulCommandHandler<CalculatorState>, BaseStatefulQueryHandler<CalculatorState>, T> builder) {
        return builder.Tell<SetValueState2Type, double>();
    }
}


public static class ExtensionsStatefulCommand3Types
{
    public static IGrainCommandBuilder<ITransactionChildGrain<CalculatorState>, BaseStatefulCommandHandler<CalculatorState>, BaseStatefulQueryHandler<CalculatorState>, double> SetValueState3Type(this IGrainCommandBaseBuilder<ITransactionChildGrain<CalculatorState>, BaseStatefulCommandHandler<CalculatorState>, BaseStatefulQueryHandler<CalculatorState>> builder, int input) 
    {
        return builder.Tell<SetValueState3Type, int, double>(input);
    }

    public static IGrainCommandBuilder<ITransactionChildGrain<CalculatorState>, BaseStatefulCommandHandler<CalculatorState>, BaseStatefulQueryHandler<CalculatorState>, double> SetValueState3Type<T>(this IGrainCommandBuilder<ITransactionChildGrain<CalculatorState>, BaseStatefulCommandHandler<CalculatorState>, BaseStatefulQueryHandler<CalculatorState>, T> builder, int input) {
        return builder.Tell<SetValueState3Type, int, double>(input);
    }

    public static IGrainCommandBuilder<ITransactionChildGrain<CalculatorState>, BaseStatefulCommandHandler<CalculatorState>, BaseStatefulQueryHandler<CalculatorState>, double> ThenSetValueState3Type(this IGrainCommandBuilder<ITransactionChildGrain<CalculatorState>, BaseStatefulCommandHandler<CalculatorState>, BaseStatefulQueryHandler<CalculatorState>, int> builder) {
        return builder.ThenTell<SetValueState3Type, double>();
    }
}

public static class ExtensionsStatefulQuery2Types
{
    public static IGrainQueryBuilder<ITransactionChildGrain<CalculatorState>, BaseStatefulQueryHandler<CalculatorState>, double> GetValueState2Type(this IGrainQueryBaseBuilder<ITransactionChildGrain<CalculatorState>, BaseStatefulQueryHandler<CalculatorState>> builder) 
    {
        return builder.Ask<GetValueState2Type, double>();
    }

    public static IGrainCommandBuilder<ITransactionChildGrain<CalculatorState>, BaseStatefulCommandHandler<CalculatorState>, BaseStatefulQueryHandler<CalculatorState>, double> GetValueState2Type(this IGrainCommandBaseBuilder<ITransactionChildGrain<CalculatorState>, BaseStatefulCommandHandler<CalculatorState>, BaseStatefulQueryHandler<CalculatorState>> builder) 
    {
        return builder.Ask<GetValueState2Type, double>();
    }

    public static IGrainQueryBuilder<ITransactionChildGrain<CalculatorState>, BaseStatefulQueryHandler<CalculatorState>, double> GetValueState2Type<T>(this IGrainQueryBuilder<ITransactionChildGrain<CalculatorState>, BaseStatefulQueryHandler<CalculatorState>, T> builder) 
    {
        return builder.Ask<GetValueState2Type, double>();
    }

    public static IGrainCommandBuilder<ITransactionChildGrain<CalculatorState>, BaseStatefulCommandHandler<CalculatorState>, BaseStatefulQueryHandler<CalculatorState>, double> GetValueState2Type<T>(this IGrainCommandBuilder<ITransactionChildGrain<CalculatorState>, BaseStatefulCommandHandler<CalculatorState>, BaseStatefulQueryHandler<CalculatorState>, T> builder) 
    {
        return builder.Ask<GetValueState2Type, double>();
    }
}


public static class ExtensionsStatefulQuery3Types
{
    public static IGrainQueryBuilder<ITransactionChildGrain<CalculatorState>, BaseStatefulQueryHandler<CalculatorState>, double> GetValueState2Type(this IGrainQueryBaseBuilder<ITransactionChildGrain<CalculatorState>, BaseStatefulQueryHandler<CalculatorState>> builder, int input) 
    {
        return builder.Ask<GetValueState3Type, int, double>(input);
    }

    public static IGrainCommandBuilder<ITransactionChildGrain<CalculatorState>, BaseStatefulCommandHandler<CalculatorState>, BaseStatefulQueryHandler<CalculatorState>, double> GetValueState2Type(this IGrainCommandBaseBuilder<ITransactionChildGrain<CalculatorState>, BaseStatefulCommandHandler<CalculatorState>, BaseStatefulQueryHandler<CalculatorState>> builder, int input) 
    {
        return builder.Ask<GetValueState3Type, int, double>(input);
    }

    public static IGrainQueryBuilder<ITransactionChildGrain<CalculatorState>, BaseStatefulQueryHandler<CalculatorState>, double> GetValueState2Type<T>(this IGrainQueryBuilder<ITransactionChildGrain<CalculatorState>, BaseStatefulQueryHandler<CalculatorState>, T> builder, int input) 
    {
        return builder.Ask<GetValueState3Type, int, double>(input);
    }
    
    public static IGrainCommandBuilder<ITransactionChildGrain<CalculatorState>, BaseStatefulCommandHandler<CalculatorState>, BaseStatefulQueryHandler<CalculatorState>, double> GetValueState2Type<T>(this IGrainCommandBuilder<ITransactionChildGrain<CalculatorState>, BaseStatefulCommandHandler<CalculatorState>, BaseStatefulQueryHandler<CalculatorState>, T> builder, int input) 
    {
        return builder.Ask<GetValueState3Type, int, double>(input);
    }

    public static IGrainQueryBuilder<ITransactionChildGrain<CalculatorState>, BaseStatefulQueryHandler<CalculatorState>, double> GetValueState2Type(this IGrainQueryBuilder<ITransactionChildGrain<CalculatorState>, BaseStatefulQueryHandler<CalculatorState>, int> builder) 
    {
        return builder.ThenAsk<GetValueState3Type, double>();
    }
    
    public static IGrainCommandBuilder<ITransactionChildGrain<CalculatorState>, BaseStatefulCommandHandler<CalculatorState>, BaseStatefulQueryHandler<CalculatorState>, double> GetValueState2Type(this IGrainCommandBuilder<ITransactionChildGrain<CalculatorState>, BaseStatefulCommandHandler<CalculatorState>, BaseStatefulQueryHandler<CalculatorState>, int> builder) 
    {
        return builder.ThenAsk<GetValueState3Type, double>();
    }
}