//HintName: TTestQuery2Type.g.cs
using System;
using System.Threading.Tasks;
using Remotr;

namespace GenericsGenerationTest;

public static class TTestQuery2Type
{
        public static IGrainQueryBuilder<IAggregateEntity<T>, BaseEntityQueryHandler<T>, K> TestQuery2Type<T, K>(this IGrainQueryBaseBuilder<IAggregateEntity<T>, BaseEntityQueryHandler<T>> builder) where K : new() where T : ITest
        {
            return builder.Ask<TestQuery2Type<T, K>, K>();
        }

        public static IGrainQueryBuilder<IAggregateEntity<T>, BaseEntityQueryHandler<T>, K> TestQuery2Type<T, K, TOther>(this IGrainQueryBuilder<IAggregateEntity<T>, BaseEntityQueryHandler<T>, TOther> builder) where K : new() where T : ITest
        {
            return builder.Ask<TestQuery2Type<T, K>, K>();
        }

        public static IGrainCommandBuilder<IAggregateEntity<T>, BaseEntityCommandHandler<T>, BaseEntityQueryHandler<T>, K> TestQuery2Type<T, K>(this IGrainCommandBaseBuilder<IAggregateEntity<T>, BaseEntityCommandHandler<T>, BaseEntityQueryHandler<T>> builder) where K : new() where T : ITest
        {
            return builder.Ask<TestQuery2Type<T, K>, K>();
        }

        public static IGrainCommandBuilder<IAggregateEntity<T>, BaseEntityCommandHandler<T>, BaseEntityQueryHandler<T>, K> TestQuery2Type<T, K, TOther>(this IGrainCommandBuilder<IAggregateEntity<T>, BaseEntityCommandHandler<T>, BaseEntityQueryHandler<T>, TOther> builder) where K : new() where T : ITest
        {
            return builder.Ask<TestQuery2Type<T, K>, K>();
        }
}
