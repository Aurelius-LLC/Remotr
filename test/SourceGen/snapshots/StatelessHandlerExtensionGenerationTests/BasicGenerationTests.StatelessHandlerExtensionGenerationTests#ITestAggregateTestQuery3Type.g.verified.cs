//HintName: ITestAggregateTestQuery3Type.g.cs
using System;
using System.Threading.Tasks;
using Remotr;

namespace StatelessHandlerExtensionGenerationTests;

public static class ITestAggregateTestQuery3Type
{
        public static IGrainQueryBuilder<ITestAggregate, BaseRootQueryHandler<ITestAggregate>, TestOutputObject> TestQuery3Type(this IGrainQueryBaseBuilder<ITestAggregate, BaseRootQueryHandler<ITestAggregate>> builder, TestInputObject input)
        {
            return builder.Ask<TestQuery3Type, TestInputObject, TestOutputObject>(input);
        }

        public static IGrainQueryBuilder<ITestAggregate, BaseRootQueryHandler<ITestAggregate>, TestOutputObject> TestQuery3Type<T>(this IGrainQueryBuilder<ITestAggregate, BaseRootQueryHandler<ITestAggregate>, T> builder, TestInputObject input)
        {
            return builder.Ask<TestQuery3Type, TestInputObject, TestOutputObject>(input);
        }

        public static IGrainQueryBuilder<ITestAggregate, BaseRootQueryHandler<ITestAggregate>, TestOutputObject> ThenTestQuery3Type(this IGrainQueryBuilder<ITestAggregate, BaseRootQueryHandler<ITestAggregate>, TestInputObject> builder)
        {
            return builder.ThenAsk<TestQuery3Type, TestOutputObject>();
        }

        public static IGrainCommandBuilder<ITestAggregate, BaseRootCommandHandler<ITestAggregate>, BaseRootQueryHandler<ITestAggregate>, TestOutputObject> TestQuery3Type(this IGrainCommandBaseBuilder<ITestAggregate, BaseRootCommandHandler<ITestAggregate>, BaseRootQueryHandler<ITestAggregate>> builder, TestInputObject input)
        {
            return builder.Ask<TestQuery3Type, TestInputObject, TestOutputObject>(input);
        }

        public static IGrainCommandBuilder<ITestAggregate, BaseRootCommandHandler<ITestAggregate>, BaseRootQueryHandler<ITestAggregate>, TestOutputObject> TestQuery3Type<T>(this IGrainCommandBuilder<ITestAggregate, BaseRootCommandHandler<ITestAggregate>, BaseRootQueryHandler<ITestAggregate>, T> builder, TestInputObject input)
        {
            return builder.Ask<TestQuery3Type, TestInputObject, TestOutputObject>(input);
        }

        public static IGrainCommandBuilder<ITestAggregate, BaseRootCommandHandler<ITestAggregate>, BaseRootQueryHandler<ITestAggregate>, TestOutputObject> ThenTestQuery3Type(this IGrainCommandBuilder<ITestAggregate, BaseRootCommandHandler<ITestAggregate>, BaseRootQueryHandler<ITestAggregate>, TestInputObject> builder)
        {
            return builder.ThenAsk<TestQuery3Type, TestOutputObject>();
        }
}
