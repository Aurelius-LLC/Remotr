//HintName: TestStateTestQuery3Type.g.cs
using System;
using System.Threading.Tasks;
using Remotr;

namespace CommandsAndQueriesWithObjectsGenerationTest;

public static class TestStateTestQuery3Type
{
        public static IGrainQueryBuilder<IAggregateEntity<TestState>, BaseEntityQueryHandler<TestState>, TestOutputObject> TestQuery3Type(this IGrainQueryBaseBuilder<IAggregateEntity<TestState>, BaseEntityQueryHandler<TestState>> builder, TestInputObject input)
        {
            return builder.Ask<TestQuery3Type, TestInputObject, TestOutputObject>(input);
        }

        public static IGrainQueryBuilder<IAggregateEntity<TestState>, BaseEntityQueryHandler<TestState>, TestOutputObject> TestQuery3Type<T>(this IGrainQueryBuilder<IAggregateEntity<TestState>, BaseEntityQueryHandler<TestState>, T> builder, TestInputObject input)
        {
            return builder.Ask<TestQuery3Type, TestInputObject, TestOutputObject>(input);
        }

        public static IGrainQueryBuilder<IAggregateEntity<TestState>, BaseEntityQueryHandler<TestState>, TestOutputObject> ThenTestQuery3Type(this IGrainQueryBuilder<IAggregateEntity<TestState>, BaseEntityQueryHandler<TestState>, TestInputObject> builder)
        {
            return builder.ThenAsk<TestQuery3Type, TestOutputObject>();
        }

        public static IGrainCommandBuilder<IAggregateEntity<TestState>, BaseEntityCommandHandler<TestState>, BaseEntityQueryHandler<TestState>, TestOutputObject> TestQuery3Type(this IGrainCommandBaseBuilder<IAggregateEntity<TestState>, BaseEntityCommandHandler<TestState>, BaseEntityQueryHandler<TestState>> builder, TestInputObject input)
        {
            return builder.Ask<TestQuery3Type, TestInputObject, TestOutputObject>(input);
        }

        public static IGrainCommandBuilder<IAggregateEntity<TestState>, BaseEntityCommandHandler<TestState>, BaseEntityQueryHandler<TestState>, TestOutputObject> TestQuery3Type<T>(this IGrainCommandBuilder<IAggregateEntity<TestState>, BaseEntityCommandHandler<TestState>, BaseEntityQueryHandler<TestState>, T> builder, TestInputObject input)
        {
            return builder.Ask<TestQuery3Type, TestInputObject, TestOutputObject>(input);
        }

        public static IGrainCommandBuilder<IAggregateEntity<TestState>, BaseEntityCommandHandler<TestState>, BaseEntityQueryHandler<TestState>, TestOutputObject> ThenTestQuery3Type(this IGrainCommandBuilder<IAggregateEntity<TestState>, BaseEntityCommandHandler<TestState>, BaseEntityQueryHandler<TestState>, TestInputObject> builder)
        {
            return builder.ThenAsk<TestQuery3Type, TestOutputObject>();
        }
}
