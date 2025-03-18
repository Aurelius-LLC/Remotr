//HintName: ITestAggregateTestCommand1Type.g.cs
using System;
using System.Threading.Tasks;
using Remotr;

namespace StatelessHandlerExtensionGenerationTests;

public static class ITestAggregateTestCommand1Type
{
        public static IGrainCommandBuilder<ITestAggregate, BaseRootCommandHandler<ITestAggregate>, BaseRootQueryHandler<ITestAggregate>, double> TestCommand1Type(this IGrainCommandBaseBuilder<ITestAggregate, BaseRootCommandHandler<ITestAggregate>, BaseRootQueryHandler<ITestAggregate>> builder, TestInputObject input)
        {
            return builder.Tell<TestCommand1Type, TestInputObject, double>(input);
        }

        public static IGrainCommandBuilder<ITestAggregate, BaseRootCommandHandler<ITestAggregate>, BaseRootQueryHandler<ITestAggregate>, double> TestCommand1Type<T>(this IGrainCommandBuilder<ITestAggregate, BaseRootCommandHandler<ITestAggregate>, BaseRootQueryHandler<ITestAggregate>, T> builder, TestInputObject input)
        {
            return builder.Tell<TestCommand1Type, TestInputObject, double>(input);
        }

        public static IGrainCommandBuilder<ITestAggregate, BaseRootCommandHandler<ITestAggregate>, BaseRootQueryHandler<ITestAggregate>, double> ThenTestCommand1Type(this IGrainCommandBuilder<ITestAggregate, BaseRootCommandHandler<ITestAggregate>, BaseRootQueryHandler<ITestAggregate>, TestInputObject> builder)
        {
            return builder.ThenTell<TestCommand1Type, double>();
        }
}
