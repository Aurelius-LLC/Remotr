//HintName: TestStateTestCommand1Type.g.cs
using System;
using System.Threading.Tasks;
using Remotr;

namespace SimpleCommandsAndQueriesTest;

public static class TestStateTestCommand1Type
{
        public static IGrainCommandBaseBuilder<IAggregateEntity<TestState>, BaseStatefulCommandHandler<TestState>, BaseStatefulQueryHandler<TestState>> TestCommand1Type(this IGrainCommandBaseBuilder<IAggregateEntity<TestState>, BaseStatefulCommandHandler<TestState>, BaseStatefulQueryHandler<TestState>> builder)
        {
            return builder.Tell<TestCommand1Type>();
        }

        public static IGrainCommandBaseBuilder<IAggregateEntity<TestState>, BaseStatefulCommandHandler<TestState>, BaseStatefulQueryHandler<TestState>> TestCommand1Type<T>(this IGrainCommandBuilder<IAggregateEntity<TestState>, BaseStatefulCommandHandler<TestState>, BaseStatefulQueryHandler<TestState>, T> builder)
        {
            return builder.Tell<TestCommand1Type>();
        }
}
