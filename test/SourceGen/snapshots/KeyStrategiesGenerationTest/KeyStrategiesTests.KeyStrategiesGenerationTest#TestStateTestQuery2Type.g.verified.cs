//HintName: TestStateTestQuery2Type.g.cs
using System;
using System.Threading.Tasks;
using Remotr;

namespace KeyStrategiesGenerationTest;

public static class TestStateTestQuery2Type
{
        public static IGrainQueryBuilder<ITransactionChildGrain<TestState>, BaseStatefulQueryHandler<TestState>, double> TestQuery2Type(this IGrainQueryBaseBuilder<ITransactionChildGrain<TestState>, BaseStatefulQueryHandler<TestState>> builder)
        {
            return builder.Ask<TestQuery2Type, double>();
        }

        public static IGrainQueryBuilder<ITransactionChildGrain<TestState>, BaseStatefulQueryHandler<TestState>, double> TestQuery2Type<T>(this IGrainQueryBuilder<ITransactionChildGrain<TestState>, BaseStatefulQueryHandler<TestState>, T> builder)
        {
            return builder.Ask<TestQuery2Type, double>();
        }

        public static IGrainCommandBuilder<ITransactionChildGrain<TestState>, BaseStatefulCommandHandler<TestState>, BaseStatefulQueryHandler<TestState>, double> TestQuery2Type(this IGrainCommandBaseBuilder<ITransactionChildGrain<TestState>, BaseStatefulCommandHandler<TestState>, BaseStatefulQueryHandler<TestState>> builder)
        {
            return builder.Ask<TestQuery2Type, double>();
        }

        public static IGrainCommandBuilder<ITransactionChildGrain<TestState>, BaseStatefulCommandHandler<TestState>, BaseStatefulQueryHandler<TestState>, double> TestQuery2Type<T>(this IGrainCommandBuilder<ITransactionChildGrain<TestState>, BaseStatefulCommandHandler<TestState>, BaseStatefulQueryHandler<TestState>, T> builder)
        {
            return builder.Ask<TestQuery2Type, double>();
        }
}
