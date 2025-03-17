//HintName: ITestAggregateTestQuery3Type.g.cs
using System;
using System.Threading.Tasks;
using Remotr;

namespace StatelessHandlerExtensionGenerationTests;

public static class ITestAggregateTestQuery3Type
{
        public static IGrainQueryBuilder<ITestAggregate, BaseStatelessQueryHandler<ITestAggregate>, TestOutputObject> TestQuery3Type(this IGrainQueryBaseBuilder<ITestAggregate, BaseStatelessQueryHandler<ITestAggregate>> builder, TestInputObject input)
        {
            return builder.Ask<TestQuery3Type, TestInputObject, TestOutputObject>(input);
        }

        public static IGrainQueryBuilder<ITestAggregate, BaseStatelessQueryHandler<ITestAggregate>, TestOutputObject> TestQuery3Type<T>(this IGrainQueryBuilder<ITestAggregate, BaseStatelessQueryHandler<ITestAggregate>, T> builder, TestInputObject input)
        {
            return builder.Ask<TestQuery3Type, TestInputObject, TestOutputObject>(input);
        }

        public static IGrainQueryBuilder<ITestAggregate, BaseStatelessQueryHandler<ITestAggregate>, TestOutputObject> ThenTestQuery3Type(this IGrainQueryBuilder<ITestAggregate, BaseStatelessQueryHandler<ITestAggregate>, TestInputObject> builder)
        {
            return builder.ThenAsk<TestQuery3Type, TestOutputObject>();
        }

        public static IGrainCommandBuilder<ITestAggregate, BaseStatelessCommandHandler<ITestAggregate>, BaseStatelessQueryHandler<ITestAggregate>, TestOutputObject> TestQuery3Type(this IGrainCommandBaseBuilder<ITestAggregate, BaseStatelessCommandHandler<ITestAggregate>, BaseStatelessQueryHandler<ITestAggregate>> builder, TestInputObject input)
        {
            return builder.Ask<TestQuery3Type, TestInputObject, TestOutputObject>(input);
        }

        public static IGrainCommandBuilder<ITestAggregate, BaseStatelessCommandHandler<ITestAggregate>, BaseStatelessQueryHandler<ITestAggregate>, TestOutputObject> TestQuery3Type<T>(this IGrainCommandBuilder<ITestAggregate, BaseStatelessCommandHandler<ITestAggregate>, BaseStatelessQueryHandler<ITestAggregate>, T> builder, TestInputObject input)
        {
            return builder.Ask<TestQuery3Type, TestInputObject, TestOutputObject>(input);
        }

        public static IGrainCommandBuilder<ITestAggregate, BaseStatelessCommandHandler<ITestAggregate>, BaseStatelessQueryHandler<ITestAggregate>, TestOutputObject> ThenTestQuery3Type(this IGrainCommandBuilder<ITestAggregate, BaseStatelessCommandHandler<ITestAggregate>, BaseStatelessQueryHandler<ITestAggregate>, TestInputObject> builder)
        {
            return builder.ThenAsk<TestQuery3Type, TestOutputObject>();
        }
}
