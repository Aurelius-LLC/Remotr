﻿//HintName: TestC1.g.cs
// This file was generated by the Remotr.SourceGen HandlerAttributeGenerator.

using System;
using System.Threading.Tasks;
using Remotr;

namespace GenericsGenerationTest;

public class TestC1 : RootCommandHandler<ITestAggregate>
{
    public override async Task Execute()
    {
        await CommandFactory.GetEntity<GenericsGenerationTest.TestState>()
            .Tell<TestCommand1Type<GenericsGenerationTest.TestState>>()
            .Run(GetRootKeyString());
    }
}

public static class ITestAggregateTestC1Extensions
{
        public static IGrainCommandBaseBuilder<GenericsGenerationTest.ITestAggregate, BaseRootCommandHandler<GenericsGenerationTest.ITestAggregate>, BaseRootQueryHandler<GenericsGenerationTest.ITestAggregate>> TestC1(this IGrainCommandBaseBuilder<GenericsGenerationTest.ITestAggregate, BaseRootCommandHandler<GenericsGenerationTest.ITestAggregate>, BaseRootQueryHandler<GenericsGenerationTest.ITestAggregate>> builder)
        {
            return builder.Tell<TestC1>();
        }

        public static IGrainCommandBaseBuilder<GenericsGenerationTest.ITestAggregate, BaseRootCommandHandler<GenericsGenerationTest.ITestAggregate>, BaseRootQueryHandler<GenericsGenerationTest.ITestAggregate>> TestC1<T>(this IGrainCommandBuilder<GenericsGenerationTest.ITestAggregate, BaseRootCommandHandler<GenericsGenerationTest.ITestAggregate>, BaseRootQueryHandler<GenericsGenerationTest.ITestAggregate>, T> builder)
        {
            return builder.Tell<TestC1>();
        }
}
