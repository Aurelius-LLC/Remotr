//HintName: TTestCommand3Type.g.cs
using System;
using System.Threading.Tasks;
using Remotr;

namespace GenericsGenerationTest;

public static class TTestCommand3Type
{
        public static IGrainCommandBuilder<ITransactionChildGrain<TState>, BaseStatefulCommandHandler<TState>, BaseStatefulQueryHandler<TState>, TOut> TestCommand3Type<TState, TIn, TOut>(this IGrainCommandBaseBuilder<ITransactionChildGrain<TState>, BaseStatefulCommandHandler<TState>, BaseStatefulQueryHandler<TState>> builder, TIn input) where TState : ITest
        {
            return builder.Tell<TestCommand3Type<TState, TIn, TOut>, TIn, TOut>(input);
        }

        public static IGrainCommandBuilder<ITransactionChildGrain<TState>, BaseStatefulCommandHandler<TState>, BaseStatefulQueryHandler<TState>, TOut> TestCommand3Type<TState, TIn, TOut, T>(this IGrainCommandBuilder<ITransactionChildGrain<TState>, BaseStatefulCommandHandler<TState>, BaseStatefulQueryHandler<TState>, dynamic> builder, TIn input) where TState : ITest
        {
            return builder.Tell<TestCommand3Type<TState, TIn, TOut>, TIn, TOut>(input);
        }

        public static IGrainCommandBuilder<ITransactionChildGrain<TState>, BaseStatefulCommandHandler<TState>, BaseStatefulQueryHandler<TState>, Output> ThenTestCommand3Type<TState, Input, Output>(this IGrainCommandBuilder<ITransactionChildGrain<TState>, BaseStatefulCommandHandler<TState>, BaseStatefulQueryHandler<TState>, Input> builder) where TState : ITest
        {
            return builder.ThenTell<TestCommand3Type<TState, Input, Output>, Output>();
        }
}
