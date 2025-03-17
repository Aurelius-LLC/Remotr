//HintName: ITestAggregateTestCommand1Type.g.cs
using System;
using System.Threading.Tasks;
using Remotr;

namespace StatelessHandlerExtensionGenerationTests;

public static class ITestAggregateTestCommand1Type
{
        public static IGrainCommandBuilder<ITestAggregate, BaseStatelessCommandHandler<ITestAggregate>, BaseStatelessQueryHandler<ITestAggregate>, double> TestCommand1Type(this IGrainCommandBaseBuilder<ITestAggregate, BaseStatelessCommandHandler<ITestAggregate>, BaseStatelessQueryHandler<ITestAggregate>> builder, TestInputObject input)
        {
            return builder.Tell<TestCommand1Type, TestInputObject, double>(input);
        }

        public static IGrainCommandBuilder<ITestAggregate, BaseStatelessCommandHandler<ITestAggregate>, BaseStatelessQueryHandler<ITestAggregate>, double> TestCommand1Type<T>(this IGrainCommandBuilder<ITestAggregate, BaseStatelessCommandHandler<ITestAggregate>, BaseStatelessQueryHandler<ITestAggregate>, T> builder, TestInputObject input)
        {
            return builder.Tell<TestCommand1Type, TestInputObject, double>(input);
        }

        public static IGrainCommandBuilder<ITestAggregate, BaseStatelessCommandHandler<ITestAggregate>, BaseStatelessQueryHandler<ITestAggregate>, double> ThenTestCommand1Type(this IGrainCommandBuilder<ITestAggregate, BaseStatelessCommandHandler<ITestAggregate>, BaseStatelessQueryHandler<ITestAggregate>, TestInputObject> builder)
        {
            return builder.ThenTell<TestCommand1Type, double>();
        }
}
