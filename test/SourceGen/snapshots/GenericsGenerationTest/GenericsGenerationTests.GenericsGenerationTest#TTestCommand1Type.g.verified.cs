//HintName: TTestCommand1Type.g.cs
using System;
using System.Threading.Tasks;
using Remotr;

namespace GenericsGenerationTest;

public static class TTestCommand1Type
{
        public static IGrainCommandBaseBuilder<ITransactionChildGrain<TState>, BaseStatefulCommandHandler<TState>, BaseStatefulQueryHandler<TState>> TestCommand1Type<TState>(this IGrainCommandBaseBuilder<ITransactionChildGrain<TState>, BaseStatefulCommandHandler<TState>, BaseStatefulQueryHandler<TState>> builder) where TState : ITest
        {
            return builder.Tell<TestCommand1Type<TState>>();
        }

        public static IGrainCommandBaseBuilder<ITransactionChildGrain<TState>, BaseStatefulCommandHandler<TState>, BaseStatefulQueryHandler<TState>> TestCommand1Type<TState, T>(this IGrainCommandBuilder<ITransactionChildGrain<TState>, BaseStatefulCommandHandler<TState>, BaseStatefulQueryHandler<TState>, T> builder) where TState : ITest
        {
            return builder.Tell<TestCommand1Type<TState>>();
        }
}
