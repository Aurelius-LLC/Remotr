//HintName: ITestManagerGrainTestCommand1Type.g.cs
using System;
using System.Threading.Tasks;
using Remotr;

namespace StatelessHandlerExtensionGenerationTests;

public static class ITestManagerGrainTestCommand1Type
{
        public static IGrainCommandBuilder<ITestManagerGrain, BaseStatelessCommandHandler<ITestManagerGrain>, BaseStatelessQueryHandler<ITestManagerGrain>, double> TestCommand1Type(this IGrainCommandBaseBuilder<ITestManagerGrain, BaseStatelessCommandHandler<ITestManagerGrain>, BaseStatelessQueryHandler<ITestManagerGrain>> builder, TestInputObject input)
        {
            return builder.Tell<TestCommand1Type, TestInputObject, double>(input);
        }

        public static IGrainCommandBuilder<ITestManagerGrain, BaseStatelessCommandHandler<ITestManagerGrain>, BaseStatelessQueryHandler<ITestManagerGrain>, double> TestCommand1Type<T>(this IGrainCommandBuilder<ITestManagerGrain, BaseStatelessCommandHandler<ITestManagerGrain>, BaseStatelessQueryHandler<ITestManagerGrain>, T> builder, TestInputObject input)
        {
            return builder.Tell<TestCommand1Type, TestInputObject, double>(input);
        }

        public static IGrainCommandBuilder<ITestManagerGrain, BaseStatelessCommandHandler<ITestManagerGrain>, BaseStatelessQueryHandler<ITestManagerGrain>, double> ThenTestCommand1Type(this IGrainCommandBuilder<ITestManagerGrain, BaseStatelessCommandHandler<ITestManagerGrain>, BaseStatelessQueryHandler<ITestManagerGrain>, TestInputObject> builder)
        {
            return builder.ThenTell<TestCommand1Type, double>();
        }
}
