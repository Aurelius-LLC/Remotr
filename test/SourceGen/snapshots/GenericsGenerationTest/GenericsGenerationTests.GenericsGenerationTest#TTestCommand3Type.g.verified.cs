//HintName: TTestCommand3Type.g.cs
using System;
using System.Threading.Tasks;
using Remotr;

namespace GenericsGenerationTest;

public static class TTestCommand3Type
{
        public static IGrainCommandBuilder<IAggregateEntity<T>, BaseEntityCommandHandler<T>, BaseEntityQueryHandler<T>, V> TestCommand3Type<T, K, V>(this IGrainCommandBaseBuilder<IAggregateEntity<T>, BaseEntityCommandHandler<T>, BaseEntityQueryHandler<T>> builder, K input) where K : new() where V : new() where T : new()
        {
            return builder.Tell<TestCommand3Type<T, K, V>, K, V>(input);
        }

        public static IGrainCommandBuilder<IAggregateEntity<T>, BaseEntityCommandHandler<T>, BaseEntityQueryHandler<T>, V> TestCommand3Type<T, K, V, TOther>(this IGrainCommandBuilder<IAggregateEntity<T>, BaseEntityCommandHandler<T>, BaseEntityQueryHandler<T>, TOther> builder, K input) where K : new() where V : new() where T : new()
        {
            return builder.Tell<TestCommand3Type<T, K, V>, K, V>(input);
        }

        public static IGrainCommandBuilder<IAggregateEntity<T>, BaseEntityCommandHandler<T>, BaseEntityQueryHandler<T>, V> ThenTestCommand3Type<T, K, V>(this IGrainCommandBuilder<IAggregateEntity<T>, BaseEntityCommandHandler<T>, BaseEntityQueryHandler<T>, K> builder) where K : new() where V : new() where T : new()
        {
            return builder.ThenTell<TestCommand3Type<T, K, V>, V>();
        }
}
