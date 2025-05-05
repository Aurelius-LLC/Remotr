//HintName: TTestQuery3Type.g.cs
using System;
using System.Threading.Tasks;
using Remotr;

namespace GenericsGenerationTest;

public static class TTestQuery3Type
{
        public static IGrainQueryBuilder<IAggregateEntity<T>, BaseEntityQueryHandler<T>, V> TestQuery3Type<T, K, V>(this IGrainQueryBaseBuilder<IAggregateEntity<T>, BaseEntityQueryHandler<T>> builder, K input) where K : new() where V : new() where T : ITest
        {
            return builder.Ask<TestQuery3Type<T, K, V>, K, V>(input);
        }

        public static IGrainQueryBuilder<IAggregateEntity<T>, BaseEntityQueryHandler<T>, V> TestQuery3Type<T, K, V, TOther>(this IGrainQueryBuilder<IAggregateEntity<T>, BaseEntityQueryHandler<T>, TOther> builder, K input) where K : new() where V : new() where T : ITest
        {
            return builder.Ask<TestQuery3Type<T, K, V>, K, V>(input);
        }

        public static IGrainQueryBuilder<IAggregateEntity<T>, BaseEntityQueryHandler<T>, V> ThenTestQuery3Type<T, K, V>(this IGrainQueryBuilder<IAggregateEntity<T>, BaseEntityQueryHandler<T>, K> builder) where K : new() where V : new() where T : ITest
        {
            return builder.ThenAsk<TestQuery3Type<T, K, V>, V>();
        }

        public static IGrainCommandBuilder<IAggregateEntity<T>, BaseEntityCommandHandler<T>, BaseEntityQueryHandler<T>, V> TestQuery3Type<T, K, V>(this IGrainCommandBaseBuilder<IAggregateEntity<T>, BaseEntityCommandHandler<T>, BaseEntityQueryHandler<T>> builder, K input) where K : new() where V : new() where T : ITest
        {
            return builder.Ask<TestQuery3Type<T, K, V>, K, V>(input);
        }

        public static IGrainCommandBuilder<IAggregateEntity<T>, BaseEntityCommandHandler<T>, BaseEntityQueryHandler<T>, V> TestQuery3Type<T, K, V, TOther>(this IGrainCommandBuilder<IAggregateEntity<T>, BaseEntityCommandHandler<T>, BaseEntityQueryHandler<T>, TOther> builder, K input) where K : new() where V : new() where T : ITest
        {
            return builder.Ask<TestQuery3Type<T, K, V>, K, V>(input);
        }

        public static IGrainCommandBuilder<IAggregateEntity<T>, BaseEntityCommandHandler<T>, BaseEntityQueryHandler<T>, V> ThenTestQuery3Type<T, K, V>(this IGrainCommandBuilder<IAggregateEntity<T>, BaseEntityCommandHandler<T>, BaseEntityQueryHandler<T>, K> builder) where K : new() where V : new() where T : ITest
        {
            return builder.ThenAsk<TestQuery3Type<T, K, V>, V>();
        }
}
