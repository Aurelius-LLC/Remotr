//HintName: ITestManagerGrainTestQuery3Type.g.cs
using System;
using System.Threading.Tasks;
using Remotr;

namespace StatelessHandlerExtensionGenerationTests;

public static class ITestManagerGrainTestQuery3Type
{
        public static IGrainQueryBuilder<ITestManagerGrain, BaseStatelessQueryHandler<ITestManagerGrain>, TestOutputObject> TestQuery3Type(this IGrainQueryBaseBuilder<ITestManagerGrain, BaseStatelessQueryHandler<ITestManagerGrain>> builder, TestInputObject input)
        {
            return builder.Ask<TestQuery3Type, TestInputObject, TestOutputObject>(input);
        }

        public static IGrainQueryBuilder<ITestManagerGrain, BaseStatelessQueryHandler<ITestManagerGrain>, TestOutputObject> TestQuery3Type<T>(this IGrainQueryBuilder<ITestManagerGrain, BaseStatelessQueryHandler<ITestManagerGrain>, T> builder, TestInputObject input)
        {
            return builder.Ask<TestQuery3Type, TestInputObject, TestOutputObject>(input);
        }

        public static IGrainQueryBuilder<ITestManagerGrain, BaseStatelessQueryHandler<ITestManagerGrain>, TestOutputObject> ThenTestQuery3Type(this IGrainQueryBuilder<ITestManagerGrain, BaseStatelessQueryHandler<ITestManagerGrain>, TestInputObject> builder)
        {
            return builder.ThenAsk<TestQuery3Type, TestOutputObject>();
        }

        public static IGrainCommandBuilder<ITestManagerGrain, BaseStatelessCommandHandler<ITestManagerGrain>, BaseStatelessQueryHandler<ITestManagerGrain>, TestOutputObject> TestQuery3Type(this IGrainCommandBaseBuilder<ITestManagerGrain, BaseStatelessCommandHandler<ITestManagerGrain>, BaseStatelessQueryHandler<ITestManagerGrain>> builder, TestInputObject input)
        {
            return builder.Ask<TestQuery3Type, TestInputObject, TestOutputObject>(input);
        }

        public static IGrainCommandBuilder<ITestManagerGrain, BaseStatelessCommandHandler<ITestManagerGrain>, BaseStatelessQueryHandler<ITestManagerGrain>, TestOutputObject> TestQuery3Type<T>(this IGrainCommandBuilder<ITestManagerGrain, BaseStatelessCommandHandler<ITestManagerGrain>, BaseStatelessQueryHandler<ITestManagerGrain>, T> builder, TestInputObject input)
        {
            return builder.Ask<TestQuery3Type, TestInputObject, TestOutputObject>(input);
        }

        public static IGrainCommandBuilder<ITestManagerGrain, BaseStatelessCommandHandler<ITestManagerGrain>, BaseStatelessQueryHandler<ITestManagerGrain>, TestOutputObject> ThenTestQuery3Type(this IGrainCommandBuilder<ITestManagerGrain, BaseStatelessCommandHandler<ITestManagerGrain>, BaseStatelessQueryHandler<ITestManagerGrain>, TestInputObject> builder)
        {
            return builder.ThenAsk<TestQuery3Type, TestOutputObject>();
        }
}
