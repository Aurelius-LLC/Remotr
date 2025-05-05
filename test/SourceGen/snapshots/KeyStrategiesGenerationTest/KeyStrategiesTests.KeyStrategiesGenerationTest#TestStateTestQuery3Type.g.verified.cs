//HintName: TestStateTestQuery3Type.g.cs
using System;
using System.Threading.Tasks;
using Remotr;

namespace KeyStrategiesGenerationTest;

public static class TestStateTestQuery3Type
{
        public static IGrainQueryBuilder<IAggregateEntity<TestState>, BaseEntityQueryHandler<TestState>, double> TestQuery3Type(this IGrainQueryBaseBuilder<IAggregateEntity<TestState>, BaseEntityQueryHandler<TestState>> builder, double input)
        {
            return builder.Ask<TestQuery3Type, double, double>(input);
        }

        public static IGrainQueryBuilder<IAggregateEntity<TestState>, BaseEntityQueryHandler<TestState>, double> TestQuery3Type<T>(this IGrainQueryBuilder<IAggregateEntity<TestState>, BaseEntityQueryHandler<TestState>, T> builder, double input)
        {
            return builder.Ask<TestQuery3Type, double, double>(input);
        }

        public static IGrainQueryBuilder<IAggregateEntity<TestState>, BaseEntityQueryHandler<TestState>, double> ThenTestQuery3Type(this IGrainQueryBuilder<IAggregateEntity<TestState>, BaseEntityQueryHandler<TestState>, double> builder)
        {
            return builder.ThenAsk<TestQuery3Type, double>();
        }

        public static IGrainCommandBuilder<IAggregateEntity<TestState>, BaseEntityCommandHandler<TestState>, BaseEntityQueryHandler<TestState>, double> TestQuery3Type(this IGrainCommandBaseBuilder<IAggregateEntity<TestState>, BaseEntityCommandHandler<TestState>, BaseEntityQueryHandler<TestState>> builder, double input)
        {
            return builder.Ask<TestQuery3Type, double, double>(input);
        }

        public static IGrainCommandBuilder<IAggregateEntity<TestState>, BaseEntityCommandHandler<TestState>, BaseEntityQueryHandler<TestState>, double> TestQuery3Type<T>(this IGrainCommandBuilder<IAggregateEntity<TestState>, BaseEntityCommandHandler<TestState>, BaseEntityQueryHandler<TestState>, T> builder, double input)
        {
            return builder.Ask<TestQuery3Type, double, double>(input);
        }

        public static IGrainCommandBuilder<IAggregateEntity<TestState>, BaseEntityCommandHandler<TestState>, BaseEntityQueryHandler<TestState>, double> ThenTestQuery3Type(this IGrainCommandBuilder<IAggregateEntity<TestState>, BaseEntityCommandHandler<TestState>, BaseEntityQueryHandler<TestState>, double> builder)
        {
            return builder.ThenAsk<TestQuery3Type, double>();
        }
}
