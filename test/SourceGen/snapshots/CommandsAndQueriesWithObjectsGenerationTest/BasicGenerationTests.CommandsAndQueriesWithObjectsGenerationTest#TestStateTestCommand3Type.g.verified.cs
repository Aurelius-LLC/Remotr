//HintName: TestStateTestCommand3Type.g.cs
using System;
using System.Threading.Tasks;
using Remotr;

namespace CommandsAndQueriesWithObjectsGenerationTest;

public static class TestStateTestCommand3Type
{
        public static IGrainCommandBuilder<ITransactionChildGrain<TestState>, BaseStatefulCommandHandler<TestState>, BaseStatefulQueryHandler<TestState>, double> TestCommand3Type(this IGrainCommandBaseBuilder<ITransactionChildGrain<TestState>, BaseStatefulCommandHandler<TestState>, BaseStatefulQueryHandler<TestState>> builder, TestInputObject input)
        {
            return builder.Tell<TestCommand3Type, TestInputObject, double>(input);
        }

        public static IGrainCommandBuilder<ITransactionChildGrain<TestState>, BaseStatefulCommandHandler<TestState>, BaseStatefulQueryHandler<TestState>, double> TestCommand3Type<T>(this IGrainCommandBuilder<ITransactionChildGrain<TestState>, BaseStatefulCommandHandler<TestState>, BaseStatefulQueryHandler<TestState>, T> builder, TestInputObject input)
        {
            return builder.Tell<TestCommand3Type, TestInputObject, double>(input);
        }

        public static IGrainCommandBuilder<ITransactionChildGrain<TestState>, BaseStatefulCommandHandler<TestState>, BaseStatefulQueryHandler<TestState>, double> ThenTestCommand3Type(this IGrainCommandBuilder<ITransactionChildGrain<TestState>, BaseStatefulCommandHandler<TestState>, BaseStatefulQueryHandler<TestState>, TestInputObject> builder)
        {
            return builder.ThenTell<TestCommand3Type, double>();
        }
}
