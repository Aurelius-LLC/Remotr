//HintName: TestStateTestQuery3Type.g.cs
using System;
using System.Threading.Tasks;
using Remotr;

namespace CommandsAndQueriesWithObjectsGenerationTest;

public static class TestStateTestQuery3Type
{
        public static IGrainQueryBuilder<ITransactionChildGrain<TestState>, BaseStatefulQueryHandler<TestState>, TestOutputObject> TestQuery3Type(this IGrainQueryBaseBuilder<ITransactionChildGrain<TestState>, BaseStatefulQueryHandler<TestState>> builder, TestInputObject input)
        {
            return builder.Ask<TestQuery3Type, TestInputObject, TestOutputObject>(input);
        }

        public static IGrainQueryBuilder<ITransactionChildGrain<TestState>, BaseStatefulQueryHandler<TestState>, TestOutputObject> TestQuery3Type<T>(this IGrainQueryBuilder<ITransactionChildGrain<TestState>, BaseStatefulQueryHandler<TestState>, T> builder, TestInputObject input)
        {
            return builder.Ask<TestQuery3Type, TestInputObject, TestOutputObject>(input);
        }

        public static IGrainQueryBuilder<ITransactionChildGrain<TestState>, BaseStatefulQueryHandler<TestState>, TestOutputObject> ThenTestQuery3Type(this IGrainQueryBuilder<ITransactionChildGrain<TestState>, BaseStatefulQueryHandler<TestState>, TestInputObject> builder)
        {
            return builder.ThenAsk<TestQuery3Type, TestOutputObject>();
        }

        public static IGrainCommandBuilder<ITransactionChildGrain<TestState>, BaseStatefulCommandHandler<TestState>, BaseStatefulQueryHandler<TestState>, TestOutputObject> TestQuery3Type(this IGrainCommandBaseBuilder<ITransactionChildGrain<TestState>, BaseStatefulCommandHandler<TestState>, BaseStatefulQueryHandler<TestState>> builder, TestInputObject input)
        {
            return builder.Ask<TestQuery3Type, TestInputObject, TestOutputObject>(input);
        }

        public static IGrainCommandBuilder<ITransactionChildGrain<TestState>, BaseStatefulCommandHandler<TestState>, BaseStatefulQueryHandler<TestState>, TestOutputObject> TestQuery3Type<T>(this IGrainCommandBuilder<ITransactionChildGrain<TestState>, BaseStatefulCommandHandler<TestState>, BaseStatefulQueryHandler<TestState>, T> builder, TestInputObject input)
        {
            return builder.Ask<TestQuery3Type, TestInputObject, TestOutputObject>(input);
        }

        public static IGrainCommandBuilder<ITransactionChildGrain<TestState>, BaseStatefulCommandHandler<TestState>, BaseStatefulQueryHandler<TestState>, TestOutputObject> ThenTestQuery3Type(this IGrainCommandBuilder<ITransactionChildGrain<TestState>, BaseStatefulCommandHandler<TestState>, BaseStatefulQueryHandler<TestState>, TestInputObject> builder)
        {
            return builder.ThenAsk<TestQuery3Type, TestOutputObject>();
        }
}
