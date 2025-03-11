//HintName: TTestQuery2Type.g.cs
using System;
using System.Threading.Tasks;
using Remotr;

namespace GenericsGenerationTest;

public static class TTestQuery2Type
{
        public static IGrainQueryBuilder<ITransactionChildGrain<TState>, BaseStatefulQueryHandler<TState>, TOut> TestQuery2Type<TState, TOut>(this IGrainQueryBaseBuilder<ITransactionChildGrain<TState>, BaseStatefulQueryHandler<TState>> builder) where TState : ITest
        {
            return builder.Ask<TestQuery2Type<TState, TOut>, TOut>();
        }

        public static IGrainCommandBuilder<ITransactionChildGrain<TState>, BaseStatefulCommandHandler<TState>, BaseStatefulQueryHandler<TState>, TOut> TestQuery2Type<TState, TOut>(this IGrainCommandBaseBuilder<ITransactionChildGrain<TState>, BaseStatefulCommandHandler<TState>, BaseStatefulQueryHandler<TState>> builder) where TState : ITest
        {
            return builder.Ask<TestQuery2Type<TState, TOut>, TOut>();
        }

        public static IGrainQueryBuilder<ITransactionChildGrain<TState>, BaseStatefulQueryHandler<TState>, TOut> TestQuery2Type<TState, TOut, T>(this IGrainQueryBuilder<ITransactionChildGrain<TState>, BaseStatefulQueryHandler<TState>, T> builder) where TState : ITest
        {
            return builder.Ask<TestQuery2Type<TState, TOut>, TOut>();
        }

        public static IGrainCommandBuilder<ITransactionChildGrain<TState>, BaseStatefulCommandHandler<TState>, BaseStatefulQueryHandler<TState>, TOut> TestQuery2Type<TState, TOut, T>(this IGrainCommandBuilder<ITransactionChildGrain<TState>, BaseStatefulCommandHandler<TState>, BaseStatefulQueryHandler<TState>, T> builder) where TState : ITest
        {
            return builder.Ask<TestQuery2Type<TState, TOut>, TOut>();
        }
}
