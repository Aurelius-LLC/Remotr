//HintName: TestStateTestQuery3Type.g.cs
using System;
using System.Threading.Tasks;
using Remotr;

namespace CommandsAndQueriesWithObjectsGenerationTest;

public static class TestStateTestQuery3Type
{
        public static IGrainQueryBuilder<IAggregateEntity<TestState>, BaseStatefulQueryHandler<TestState>, TestOutputObject> TestQuery3Type(this IGrainQueryBaseBuilder<IAggregateEntity<TestState>, BaseStatefulQueryHandler<TestState>> builder, TestInputObject input)
        {
            return builder.Ask<TestQuery3Type, TestInputObject, TestOutputObject>(input);
        }

        public static IGrainQueryBuilder<IAggregateEntity<TestState>, BaseStatefulQueryHandler<TestState>, TestOutputObject> TestQuery3Type<T>(this IGrainQueryBuilder<IAggregateEntity<TestState>, BaseStatefulQueryHandler<TestState>, T> builder, TestInputObject input)
        {
            return builder.Ask<TestQuery3Type, TestInputObject, TestOutputObject>(input);
        }

        public static IGrainQueryBuilder<IAggregateEntity<TestState>, BaseStatefulQueryHandler<TestState>, TestOutputObject> ThenTestQuery3Type(this IGrainQueryBuilder<IAggregateEntity<TestState>, BaseStatefulQueryHandler<TestState>, TestInputObject> builder)
        {
            return builder.ThenAsk<TestQuery3Type, TestOutputObject>();
        }

        public static IGrainCommandBuilder<IAggregateEntity<TestState>, BaseStatefulCommandHandler<TestState>, BaseStatefulQueryHandler<TestState>, TestOutputObject> TestQuery3Type(this IGrainCommandBaseBuilder<IAggregateEntity<TestState>, BaseStatefulCommandHandler<TestState>, BaseStatefulQueryHandler<TestState>> builder, TestInputObject input)
        {
            return builder.Ask<TestQuery3Type, TestInputObject, TestOutputObject>(input);
        }

        public static IGrainCommandBuilder<IAggregateEntity<TestState>, BaseStatefulCommandHandler<TestState>, BaseStatefulQueryHandler<TestState>, TestOutputObject> TestQuery3Type<T>(this IGrainCommandBuilder<IAggregateEntity<TestState>, BaseStatefulCommandHandler<TestState>, BaseStatefulQueryHandler<TestState>, T> builder, TestInputObject input)
        {
            return builder.Ask<TestQuery3Type, TestInputObject, TestOutputObject>(input);
        }

        public static IGrainCommandBuilder<IAggregateEntity<TestState>, BaseStatefulCommandHandler<TestState>, BaseStatefulQueryHandler<TestState>, TestOutputObject> ThenTestQuery3Type(this IGrainCommandBuilder<IAggregateEntity<TestState>, BaseStatefulCommandHandler<TestState>, BaseStatefulQueryHandler<TestState>, TestInputObject> builder)
        {
            return builder.ThenAsk<TestQuery3Type, TestOutputObject>();
        }
}
