//HintName: TestStateTestCommand2Type.g.cs
using System;
using System.Threading.Tasks;
using Remotr;

namespace SimpleCommandsAndQueriesTest;

public static class TestStateTestCommand2Type
{
        public static IGrainCommandBuilder<IAggregateEntity<TestState>, BaseEntityCommandHandler<TestState>, BaseEntityQueryHandler<TestState>, double> TestCommand2Type(this IGrainCommandBaseBuilder<IAggregateEntity<TestState>, BaseEntityCommandHandler<TestState>, BaseEntityQueryHandler<TestState>> builder)
        {
            return builder.Tell<TestCommand2Type, double>();
        }

        public static IGrainCommandBuilder<IAggregateEntity<TestState>, BaseEntityCommandHandler<TestState>, BaseEntityQueryHandler<TestState>, double> TestCommand2Type<T>(this IGrainCommandBuilder<IAggregateEntity<TestState>, BaseEntityCommandHandler<TestState>, BaseEntityQueryHandler<TestState>, T> builder)
        {
            return builder.Tell<TestCommand2Type, double>();
        }
}
