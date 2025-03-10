//HintName: TestStateTestQuery3Type.g.cs
using System;
using System.Threading.Tasks;
using Remotr;

namespace SimpleCommandsAndQueriesTest;

public static class TestStateTestQuery3Type
{
        public static IGrainQueryBuilder<ITransactionChildGrain<TestState>, BaseStatefulQueryHandler<TestState>, double> TestQuery3Type(this IGrainQueryBaseBuilder<ITransactionChildGrain<TestState>, BaseStatefulQueryHandler<TestState>> builder, int input)
        {
            return builder.Ask<TestQuery3Type, int, double>(input);
        }

        public static IGrainCommandBuilder<ITransactionChildGrain<TestState>, BaseStatefulCommandHandler<TestState>, BaseStatefulQueryHandler<TestState>, double> TestQuery3Type(this IGrainCommandBaseBuilder<ITransactionChildGrain<TestState>, BaseStatefulCommandHandler<TestState>, BaseStatefulQueryHandler<TestState>> builder, int input)
        {
            return builder.Ask<TestQuery3Type, int, double>(input);
        }

        public static IGrainQueryBuilder<ITransactionChildGrain<TestState>, BaseStatefulQueryHandler<TestState>, double> TestQuery3Type<T>(this IGrainQueryBuilder<ITransactionChildGrain<TestState>, BaseStatefulQueryHandler<TestState>, T> builder, int input)
        {
            return builder.Ask<TestQuery3Type, int, double>(input);
        }

        public static IGrainCommandBuilder<ITransactionChildGrain<TestState>, BaseStatefulCommandHandler<TestState>, BaseStatefulQueryHandler<TestState>, double> TestQuery3Type<T>(this IGrainCommandBuilder<ITransactionChildGrain<TestState>, BaseStatefulCommandHandler<TestState>, BaseStatefulQueryHandler<TestState>, T> builder, int input)
        {
            return builder.Ask<TestQuery3Type, int, double>(input);
        }

        public static IGrainQueryBuilder<ITransactionChildGrain<TestState>, BaseStatefulQueryHandler<TestState>, double> ThenTestQuery3Type(this IGrainQueryBuilder<ITransactionChildGrain<TestState>, BaseStatefulQueryHandler<TestState>, int> builder)
        {
            return builder.ThenAsk<TestQuery3Type, double>();
        }

        public static IGrainCommandBuilder<ITransactionChildGrain<TestState>, BaseStatefulCommandHandler<TestState>, BaseStatefulQueryHandler<TestState>, double> ThenTestQuery3Type(this IGrainCommandBuilder<ITransactionChildGrain<TestState>, BaseStatefulCommandHandler<TestState>, BaseStatefulQueryHandler<TestState>, int> builder)
        {
            return builder.ThenAsk<TestQuery3Type, double>();
        }
}
