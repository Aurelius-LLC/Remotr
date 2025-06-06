﻿//HintName: TestC2.g.cs
// This file was generated by the Remotr.SourceGen HandlerAttributeGenerator.

using System;
using System.Threading.Tasks;
using Remotr;

namespace SimpleCommandsAndQueriesTest;

public class TestC2 : RootCommandHandler<ITestAggregate, double>
{
    public override async Task<double> Execute()
    {
        return await CommandFactory.GetEntity<SimpleCommandsAndQueriesTest.TestState>()
            .Tell<TestCommand2Type, double>()
            .Run(GetRootKeyString());
    }
}

public static class ITestAggregateTestC2Extensions
{
        public static IGrainCommandBuilder<SimpleCommandsAndQueriesTest.ITestAggregate, BaseRootCommandHandler<SimpleCommandsAndQueriesTest.ITestAggregate>, BaseRootQueryHandler<SimpleCommandsAndQueriesTest.ITestAggregate>, double> TestC2(this IGrainCommandBaseBuilder<SimpleCommandsAndQueriesTest.ITestAggregate, BaseRootCommandHandler<SimpleCommandsAndQueriesTest.ITestAggregate>, BaseRootQueryHandler<SimpleCommandsAndQueriesTest.ITestAggregate>> builder)
        {
            return builder.Tell<TestC2, double>();
        }

        public static IGrainCommandBuilder<SimpleCommandsAndQueriesTest.ITestAggregate, BaseRootCommandHandler<SimpleCommandsAndQueriesTest.ITestAggregate>, BaseRootQueryHandler<SimpleCommandsAndQueriesTest.ITestAggregate>, double> TestC2<T>(this IGrainCommandBuilder<SimpleCommandsAndQueriesTest.ITestAggregate, BaseRootCommandHandler<SimpleCommandsAndQueriesTest.ITestAggregate>, BaseRootQueryHandler<SimpleCommandsAndQueriesTest.ITestAggregate>, T> builder)
        {
            return builder.Tell<TestC2, double>();
        }
}
