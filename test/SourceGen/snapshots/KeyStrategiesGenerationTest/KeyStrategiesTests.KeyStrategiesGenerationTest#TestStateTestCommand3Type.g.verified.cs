//HintName: TestStateTestCommand3Type.g.cs
using System;
using System.Threading.Tasks;
using Remotr;

namespace KeyStrategiesGenerationTest;

public static class TestStateTestCommand3Type
{
        public static IGrainCommandBuilder<IAggregateEntity<TestState>, BaseEntityCommandHandler<TestState>, BaseEntityQueryHandler<TestState>, double> TestCommand3Type(this IGrainCommandBaseBuilder<IAggregateEntity<TestState>, BaseEntityCommandHandler<TestState>, BaseEntityQueryHandler<TestState>> builder, TestInputObject input)
        {
            return builder.Tell<TestCommand3Type, TestInputObject, double>(input);
        }

        public static IGrainCommandBuilder<IAggregateEntity<TestState>, BaseEntityCommandHandler<TestState>, BaseEntityQueryHandler<TestState>, double> TestCommand3Type<T>(this IGrainCommandBuilder<IAggregateEntity<TestState>, BaseEntityCommandHandler<TestState>, BaseEntityQueryHandler<TestState>, T> builder, TestInputObject input)
        {
            return builder.Tell<TestCommand3Type, TestInputObject, double>(input);
        }

        public static IGrainCommandBuilder<IAggregateEntity<TestState>, BaseEntityCommandHandler<TestState>, BaseEntityQueryHandler<TestState>, double> ThenTestCommand3Type(this IGrainCommandBuilder<IAggregateEntity<TestState>, BaseEntityCommandHandler<TestState>, BaseEntityQueryHandler<TestState>, TestInputObject> builder)
        {
            return builder.ThenTell<TestCommand3Type, double>();
        }
}
