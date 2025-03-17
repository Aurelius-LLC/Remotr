//HintName: TestStateTestCommand3Type.g.cs
using System;
using System.Threading.Tasks;
using Remotr;

namespace SimpleCommandsAndQueriesTest;

public static class TestStateTestCommand3Type
{
        public static IGrainCommandBuilder<IAggregateEntity<TestState>, BaseStatefulCommandHandler<TestState>, BaseStatefulQueryHandler<TestState>, double> TestCommand3Type(this IGrainCommandBaseBuilder<IAggregateEntity<TestState>, BaseStatefulCommandHandler<TestState>, BaseStatefulQueryHandler<TestState>> builder, int input)
        {
            return builder.Tell<TestCommand3Type, int, double>(input);
        }

        public static IGrainCommandBuilder<IAggregateEntity<TestState>, BaseStatefulCommandHandler<TestState>, BaseStatefulQueryHandler<TestState>, double> TestCommand3Type<T>(this IGrainCommandBuilder<IAggregateEntity<TestState>, BaseStatefulCommandHandler<TestState>, BaseStatefulQueryHandler<TestState>, T> builder, int input)
        {
            return builder.Tell<TestCommand3Type, int, double>(input);
        }

        public static IGrainCommandBuilder<IAggregateEntity<TestState>, BaseStatefulCommandHandler<TestState>, BaseStatefulQueryHandler<TestState>, double> ThenTestCommand3Type(this IGrainCommandBuilder<IAggregateEntity<TestState>, BaseStatefulCommandHandler<TestState>, BaseStatefulQueryHandler<TestState>, int> builder)
        {
            return builder.ThenTell<TestCommand3Type, double>();
        }
}
