//HintName: TestStateTestCommand1Type.g.cs
using System;
using System.Threading.Tasks;
using Remotr;

namespace SimpleCommandsAndQueriesTest;

public static class TestStateTestCommand1Type
{
        public static IGrainCommandBaseBuilder<IAggregateEntity<TestState>, BaseEntityCommandHandler<TestState>, BaseEntityQueryHandler<TestState>> TestCommand1Type(this IGrainCommandBaseBuilder<IAggregateEntity<TestState>, BaseEntityCommandHandler<TestState>, BaseEntityQueryHandler<TestState>> builder)
        {
            return builder.Tell<TestCommand1Type>();
        }

        public static IGrainCommandBaseBuilder<IAggregateEntity<TestState>, BaseEntityCommandHandler<TestState>, BaseEntityQueryHandler<TestState>> TestCommand1Type<T>(this IGrainCommandBuilder<IAggregateEntity<TestState>, BaseEntityCommandHandler<TestState>, BaseEntityQueryHandler<TestState>, T> builder)
        {
            return builder.Tell<TestCommand1Type>();
        }
}
