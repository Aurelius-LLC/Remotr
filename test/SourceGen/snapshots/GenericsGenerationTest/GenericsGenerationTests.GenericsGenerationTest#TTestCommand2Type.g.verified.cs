//HintName: TTestCommand2Type.g.cs
using System;
using System.Threading.Tasks;
using Remotr;

namespace GenericsGenerationTest;

public static class TTestCommand2Type
{
        public static IGrainCommandBuilder<ITransactionChildGrain<TState>, BaseStatefulCommandHandler<TState>, BaseStatefulQueryHandler<TState>, TOut> TestCommand2Type<TState, TOut>(this IGrainCommandBaseBuilder<ITransactionChildGrain<TState>, BaseStatefulCommandHandler<TState>, BaseStatefulQueryHandler<TState>> builder) where TState : ITest
        {
            return builder.Tell<TestCommand2Type<TState, TOut>, TOut>();
        }

        public static IGrainCommandBuilder<ITransactionChildGrain<TState>, BaseStatefulCommandHandler<TState>, BaseStatefulQueryHandler<TState>, TOut> TestCommand2Type<TState, TOut, T>(this IGrainCommandBuilder<ITransactionChildGrain<TState>, BaseStatefulCommandHandler<TState>, BaseStatefulQueryHandler<TState>, T> builder) where TState : ITest
        {
            return builder.Tell<TestCommand2Type<TState, TOut>, TOut>();
        }
}
