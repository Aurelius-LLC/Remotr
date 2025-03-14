//HintName: TTestCommand1Type.g.cs
using System;
using System.Threading.Tasks;
using Remotr;

namespace GenericsGenerationTest;

public static class TTestCommand1Type
{
        public static IGrainCommandBaseBuilder<ITransactionChildGrain<T>, BaseStatefulCommandHandler<T>, BaseStatefulQueryHandler<T>> TestCommand1Type<T>(this IGrainCommandBaseBuilder<ITransactionChildGrain<T>, BaseStatefulCommandHandler<T>, BaseStatefulQueryHandler<T>> builder) where T : ITest
        {
            return builder.Tell<TestCommand1Type<T>>();
        }

        public static IGrainCommandBaseBuilder<ITransactionChildGrain<T>, BaseStatefulCommandHandler<T>, BaseStatefulQueryHandler<T>> TestCommand1Type<T, TOther>(this IGrainCommandBuilder<ITransactionChildGrain<T>, BaseStatefulCommandHandler<T>, BaseStatefulQueryHandler<T>, TOther> builder) where T : ITest
        {
            return builder.Tell<TestCommand1Type<T>>();
        }
}
