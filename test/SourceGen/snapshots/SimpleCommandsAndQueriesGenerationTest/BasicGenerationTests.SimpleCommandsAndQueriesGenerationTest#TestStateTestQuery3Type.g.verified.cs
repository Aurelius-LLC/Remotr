//HintName: TestStateTestQuery3Type.g.cs
using System;
using System.Threading.Tasks;
using Remotr;

namespace SimpleCommandsAndQueriesTest;

public static class TestStateTestQuery3Type
{
        public static IGrainQueryBuilder<IAggregateEntity<TestState>, BaseStatefulQueryHandler<TestState>, double> TestQuery3Type(this IGrainQueryBaseBuilder<IAggregateEntity<TestState>, BaseStatefulQueryHandler<TestState>> builder, int input)
        {
            return builder.Ask<TestQuery3Type, int, double>(input);
        }

        public static IGrainQueryBuilder<IAggregateEntity<TestState>, BaseStatefulQueryHandler<TestState>, double> TestQuery3Type<T>(this IGrainQueryBuilder<IAggregateEntity<TestState>, BaseStatefulQueryHandler<TestState>, T> builder, int input)
        {
            return builder.Ask<TestQuery3Type, int, double>(input);
        }

        public static IGrainQueryBuilder<IAggregateEntity<TestState>, BaseStatefulQueryHandler<TestState>, double> ThenTestQuery3Type(this IGrainQueryBuilder<IAggregateEntity<TestState>, BaseStatefulQueryHandler<TestState>, int> builder)
        {
            return builder.ThenAsk<TestQuery3Type, double>();
        }

        public static IGrainCommandBuilder<IAggregateEntity<TestState>, BaseStatefulCommandHandler<TestState>, BaseStatefulQueryHandler<TestState>, double> TestQuery3Type(this IGrainCommandBaseBuilder<IAggregateEntity<TestState>, BaseStatefulCommandHandler<TestState>, BaseStatefulQueryHandler<TestState>> builder, int input)
        {
            return builder.Ask<TestQuery3Type, int, double>(input);
        }

        public static IGrainCommandBuilder<IAggregateEntity<TestState>, BaseStatefulCommandHandler<TestState>, BaseStatefulQueryHandler<TestState>, double> TestQuery3Type<T>(this IGrainCommandBuilder<IAggregateEntity<TestState>, BaseStatefulCommandHandler<TestState>, BaseStatefulQueryHandler<TestState>, T> builder, int input)
        {
            return builder.Ask<TestQuery3Type, int, double>(input);
        }

        public static IGrainCommandBuilder<IAggregateEntity<TestState>, BaseStatefulCommandHandler<TestState>, BaseStatefulQueryHandler<TestState>, double> ThenTestQuery3Type(this IGrainCommandBuilder<IAggregateEntity<TestState>, BaseStatefulCommandHandler<TestState>, BaseStatefulQueryHandler<TestState>, int> builder)
        {
            return builder.ThenAsk<TestQuery3Type, double>();
        }
}
