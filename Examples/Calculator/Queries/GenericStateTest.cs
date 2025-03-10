
namespace Remotr.Example.Calculator;


// [RemotrGen]
public class GenericStateTest<T, K> : StatefulQueryHandler<T, K> where K : new() where T : new()
{
    public override Task<K> Execute()
    {
        return Task.FromResult(new K());
    }
}

public static class TGenericStateTest
{
        public static IGrainQueryBuilder<ITransactionChildGrain<T>, BaseStatefulQueryHandler<T>, K> GenericStateTest<T, K>(this IGrainQueryBaseBuilder<ITransactionChildGrain<T>, BaseStatefulQueryHandler<T>> builder) where K : new() where T : new()
        {
            return builder.Ask<GenericStateTest<T, K>, K>();
        }

        // public static IGrainCommandBuilder<ITransactionChildGrain<T>, BaseStatefulCommandHandler<T>, BaseStatefulQueryHandler<T>, K> GenericStateTest(this IGrainCommandBaseBuilder<ITransactionChildGrain<T>, BaseStatefulCommandHandler<T>, BaseStatefulQueryHandler<T>> builder)
        // {
        //     return builder.Ask<GenericStateTest, K>();
        // }

        // public static IGrainQueryBuilder<ITransactionChildGrain<T>, BaseStatefulQueryHandler<T>, K> GenericStateTest<T>(this IGrainQueryBuilder<ITransactionChildGrain<T>, BaseStatefulQueryHandler<T>, T> builder)
        // {
        //     return builder.Ask<GenericStateTest, K>();
        // }

        // public static IGrainCommandBuilder<ITransactionChildGrain<T>, BaseStatefulCommandHandler<T>, BaseStatefulQueryHandler<T>, K> GenericStateTest<T>(this IGrainCommandBuilder<ITransactionChildGrain<T>, BaseStatefulCommandHandler<T>, BaseStatefulQueryHandler<T>, T> builder)
        // {
        //     return builder.Ask<GenericStateTest, K>();
        // }
}