//HintName: TTestCommand2Type.g.cs
using System;
using System.Threading.Tasks;
using Remotr;

namespace GenericsGenerationTest;

public static class TTestCommand2Type
{
        public static IGrainCommandBuilder<IAggregateEntity<T>, BaseEntityCommandHandler<T>, BaseEntityQueryHandler<T>, K> TestCommand2Type<T, K>(this IGrainCommandBaseBuilder<IAggregateEntity<T>, BaseEntityCommandHandler<T>, BaseEntityQueryHandler<T>> builder) where K : new() where T : ITest
        {
            return builder.Tell<TestCommand2Type<T, K>, K>();
        }

        public static IGrainCommandBuilder<IAggregateEntity<T>, BaseEntityCommandHandler<T>, BaseEntityQueryHandler<T>, K> TestCommand2Type<T, K, TOther>(this IGrainCommandBuilder<IAggregateEntity<T>, BaseEntityCommandHandler<T>, BaseEntityQueryHandler<T>, TOther> builder) where K : new() where T : ITest
        {
            return builder.Tell<TestCommand2Type<T, K>, K>();
        }
}
