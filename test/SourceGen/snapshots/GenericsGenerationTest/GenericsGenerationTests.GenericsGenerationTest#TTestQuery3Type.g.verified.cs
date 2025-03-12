//HintName: TTestQuery3Type.g.cs
using System;
using System.Threading.Tasks;
using Remotr;

namespace GenericsGenerationTest;

public static class TTestQuery3Type
{
        public static IGrainQueryBuilder<ITransactionChildGrain<TState>, BaseStatefulQueryHandler<TState>, TOut> TestQuery3Type<TState, TIn, TOut>(this IGrainQueryBaseBuilder<ITransactionChildGrain<TState>, BaseStatefulQueryHandler<TState>> builder, TIn input) where TState : ITest
        {
            return builder.Ask<TestQuery3Type<TState, TIn, TOut>, TIn, TOut>(input);
        }

        public static IGrainCommandBuilder<ITransactionChildGrain<TState>, BaseStatefulCommandHandler<TState>, BaseStatefulQueryHandler<TState>, TOut> TestQuery3Type<TState, TIn, TOut>(this IGrainCommandBaseBuilder<ITransactionChildGrain<TState>, BaseStatefulCommandHandler<TState>, BaseStatefulQueryHandler<TState>> builder, TIn input) where TState : ITest
        {
            return builder.Ask<TestQuery3Type<TState, TIn, TOut>, TIn, TOut>(input);
        }

        public static IGrainQueryBuilder<ITransactionChildGrain<TState>, BaseStatefulQueryHandler<TState>, TOut> TestQuery3Type<TState, TIn, TOut, T>(this IGrainQueryBuilder<ITransactionChildGrain<TState>, BaseStatefulQueryHandler<TState>, T> builder, TIn input) where TState : ITest
        {
            return builder.Ask<TestQuery3Type<TState, TIn, TOut>, TIn, TOut>(input);
        }

        public static IGrainCommandBuilder<ITransactionChildGrain<TState>, BaseStatefulCommandHandler<TState>, BaseStatefulQueryHandler<TState>, TOut> TestQuery3Type<TState, TIn, TOut, T>(this IGrainCommandBuilder<ITransactionChildGrain<TState>, BaseStatefulCommandHandler<TState>, BaseStatefulQueryHandler<TState>, T> builder, TIn input) where TState : ITest
        {
            return builder.Ask<TestQuery3Type<TState, TIn, TOut>, TIn, TOut>(input);
        }

        public static IGrainQueryBuilder<ITransactionChildGrain<TState>, BaseStatefulQueryHandler<TState>, TOut> ThenTestQuery3Type<TState, TIn, TOut>(this IGrainQueryBuilder<ITransactionChildGrain<TState>, BaseStatefulQueryHandler<TState>, TIn> builder)
        {
            return builder.ThenAsk<TestQuery3Type<TState, TIn, TOut>, TOut>();
        }

        public static IGrainCommandBuilder<ITransactionChildGrain<TState>, BaseStatefulCommandHandler<TState>, BaseStatefulQueryHandler<TState>, TOut> ThenTestQuery3Type<TState, TIn, TOut>(this IGrainCommandBuilder<ITransactionChildGrain<TState>, BaseStatefulCommandHandler<TState>, BaseStatefulQueryHandler<TState>, TIn> builder)
        {
            return builder.ThenAsk<TestQuery3Type<TState, TIn, TOut>, TOut>();
        }
}
