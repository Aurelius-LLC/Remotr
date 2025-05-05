//HintName: TestStateTestQuery2Type.g.cs
using System;
using System.Threading.Tasks;
using Remotr;

namespace KeyStrategiesGenerationTest;

public static class TestStateTestQuery2Type
{
        public static IGrainQueryBuilder<IAggregateEntity<TestState>, BaseEntityQueryHandler<TestState>, double> TestQuery2Type(this IGrainQueryBaseBuilder<IAggregateEntity<TestState>, BaseEntityQueryHandler<TestState>> builder)
        {
            return builder.Ask<TestQuery2Type, double>();
        }

        public static IGrainQueryBuilder<IAggregateEntity<TestState>, BaseEntityQueryHandler<TestState>, double> TestQuery2Type<T>(this IGrainQueryBuilder<IAggregateEntity<TestState>, BaseEntityQueryHandler<TestState>, T> builder)
        {
            return builder.Ask<TestQuery2Type, double>();
        }

        public static IGrainCommandBuilder<IAggregateEntity<TestState>, BaseEntityCommandHandler<TestState>, BaseEntityQueryHandler<TestState>, double> TestQuery2Type(this IGrainCommandBaseBuilder<IAggregateEntity<TestState>, BaseEntityCommandHandler<TestState>, BaseEntityQueryHandler<TestState>> builder)
        {
            return builder.Ask<TestQuery2Type, double>();
        }

        public static IGrainCommandBuilder<IAggregateEntity<TestState>, BaseEntityCommandHandler<TestState>, BaseEntityQueryHandler<TestState>, double> TestQuery2Type<T>(this IGrainCommandBuilder<IAggregateEntity<TestState>, BaseEntityCommandHandler<TestState>, BaseEntityQueryHandler<TestState>, T> builder)
        {
            return builder.Ask<TestQuery2Type, double>();
        }
}
