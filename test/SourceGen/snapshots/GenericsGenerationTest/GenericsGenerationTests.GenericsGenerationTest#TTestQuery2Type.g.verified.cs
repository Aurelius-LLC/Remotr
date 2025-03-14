//HintName: TTestQuery2Type.g.cs
using System;
using System.Threading.Tasks;
using Remotr;

namespace GenericsGenerationTest;

public static class TTestQuery2Type
{
        public static IGrainQueryBuilder<ITransactionChildGrain<T>, BaseStatefulQueryHandler<T>, K> TestQuery2Type<T, K>(this IGrainQueryBaseBuilder<ITransactionChildGrain<T>, BaseStatefulQueryHandler<T>> builder) where K : new() where T : ITest
        {
            return builder.Ask<TestQuery2Type<T, K>, K>();
        }

        public static IGrainQueryBuilder<ITransactionChildGrain<T>, BaseStatefulQueryHandler<T>, K> TestQuery2Type<T, K, TOther>(this IGrainQueryBuilder<ITransactionChildGrain<T>, BaseStatefulQueryHandler<T>, TOther> builder) where K : new() where T : ITest
        {
            return builder.Ask<TestQuery2Type<T, K>, K>();
        }

        public static IGrainCommandBuilder<ITransactionChildGrain<T>, BaseStatefulCommandHandler<T>, BaseStatefulQueryHandler<T>, K> TestQuery2Type<T, K>(this IGrainCommandBaseBuilder<ITransactionChildGrain<T>, BaseStatefulCommandHandler<T>, BaseStatefulQueryHandler<T>> builder) where K : new() where T : ITest
        {
            return builder.Ask<TestQuery2Type<T, K>, K>();
        }

        public static IGrainCommandBuilder<ITransactionChildGrain<T>, BaseStatefulCommandHandler<T>, BaseStatefulQueryHandler<T>, K> TestQuery2Type<T, K, TOther>(this IGrainCommandBuilder<ITransactionChildGrain<T>, BaseStatefulCommandHandler<T>, BaseStatefulQueryHandler<T>, TOther> builder) where K : new() where T : ITest
        {
            return builder.Ask<TestQuery2Type<T, K>, K>();
        }
}
