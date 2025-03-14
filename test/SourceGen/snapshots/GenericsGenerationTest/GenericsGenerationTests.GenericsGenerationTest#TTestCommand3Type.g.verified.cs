//HintName: TTestCommand3Type.g.cs
using System;
using System.Threading.Tasks;
using Remotr;

namespace GenericsGenerationTest;

public static class TTestCommand3Type
{
        public static IGrainCommandBuilder<ITransactionChildGrain<T>, BaseStatefulCommandHandler<T>, BaseStatefulQueryHandler<T>, V> TestCommand3Type<T, K, V>(this IGrainCommandBaseBuilder<ITransactionChildGrain<T>, BaseStatefulCommandHandler<T>, BaseStatefulQueryHandler<T>> builder, K input) where K : new() where V : new() where T : new()
        {
            return builder.Tell<TestCommand3Type<T, K, V>, K, V>(input);
        }

        public static IGrainCommandBuilder<ITransactionChildGrain<T>, BaseStatefulCommandHandler<T>, BaseStatefulQueryHandler<T>, V> TestCommand3Type<T, K, V, TOther>(this IGrainCommandBuilder<ITransactionChildGrain<T>, BaseStatefulCommandHandler<T>, BaseStatefulQueryHandler<T>, TOther> builder, K input) where K : new() where V : new() where T : new()
        {
            return builder.Tell<TestCommand3Type<T, K, V>, K, V>(input);
        }

        public static IGrainCommandBuilder<ITransactionChildGrain<T>, BaseStatefulCommandHandler<T>, BaseStatefulQueryHandler<T>, V> ThenTestCommand3Type<T, K, V>(this IGrainCommandBuilder<ITransactionChildGrain<T>, BaseStatefulCommandHandler<T>, BaseStatefulQueryHandler<T>, K> builder) where K : new() where V : new() where T : new()
        {
            return builder.ThenTell<TestCommand3Type<T, K, V>, V>();
        }
}
