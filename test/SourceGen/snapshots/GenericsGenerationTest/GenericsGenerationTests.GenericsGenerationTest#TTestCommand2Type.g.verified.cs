//HintName: TTestCommand2Type.g.cs
using System;
using System.Threading.Tasks;
using Remotr;

namespace GenericsGenerationTest;

public static class TTestCommand2Type
{
        public static IGrainCommandBuilder<ITransactionChildGrain<T>, BaseStatefulCommandHandler<T>, BaseStatefulQueryHandler<T>, K> TestCommand2Type<T, K>(this IGrainCommandBaseBuilder<ITransactionChildGrain<T>, BaseStatefulCommandHandler<T>, BaseStatefulQueryHandler<T>> builder) where T : ITest
        {
            return builder.Tell<TestCommand2Type<T, K>, K>();
        }

        public static IGrainCommandBuilder<ITransactionChildGrain<T>, BaseStatefulCommandHandler<T>, BaseStatefulQueryHandler<T>, K> TestCommand2Type<T, K, TOther>(this IGrainCommandBuilder<ITransactionChildGrain<T>, BaseStatefulCommandHandler<T>, BaseStatefulQueryHandler<T>, TOther> builder) where T : ITest
        {
            return builder.Tell<TestCommand2Type<T, K>, K>();
        }
}
