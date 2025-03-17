//HintName: TestStateTestQuery2Type.g.cs
using System;
using System.Threading.Tasks;
using Remotr;

namespace SimpleCommandsAndQueriesTest;

public static class TestStateTestQuery2Type
{
        public static IGrainQueryBuilder<IAggregateEntity<TestState>, BaseStatefulQueryHandler<TestState>, double> TestQuery2Type(this IGrainQueryBaseBuilder<IAggregateEntity<TestState>, BaseStatefulQueryHandler<TestState>> builder)
        {
            return builder.Ask<TestQuery2Type, double>();
        }

        public static IGrainQueryBuilder<IAggregateEntity<TestState>, BaseStatefulQueryHandler<TestState>, double> TestQuery2Type<T>(this IGrainQueryBuilder<IAggregateEntity<TestState>, BaseStatefulQueryHandler<TestState>, T> builder)
        {
            return builder.Ask<TestQuery2Type, double>();
        }

        public static IGrainCommandBuilder<IAggregateEntity<TestState>, BaseStatefulCommandHandler<TestState>, BaseStatefulQueryHandler<TestState>, double> TestQuery2Type(this IGrainCommandBaseBuilder<IAggregateEntity<TestState>, BaseStatefulCommandHandler<TestState>, BaseStatefulQueryHandler<TestState>> builder)
        {
            return builder.Ask<TestQuery2Type, double>();
        }

        public static IGrainCommandBuilder<IAggregateEntity<TestState>, BaseStatefulCommandHandler<TestState>, BaseStatefulQueryHandler<TestState>, double> TestQuery2Type<T>(this IGrainCommandBuilder<IAggregateEntity<TestState>, BaseStatefulCommandHandler<TestState>, BaseStatefulQueryHandler<TestState>, T> builder)
        {
            return builder.Ask<TestQuery2Type, double>();
        }
}
