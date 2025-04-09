using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Remotr;
using Remotr.Testing;
using Remotr.Samples.Calculator;

namespace RemotrTests.Tests;

[Collection(ClusterCollection.Name)]
public class ConcurrencyTests(ClusterFixture fixture)
{
    [Fact]
    public async Task TestInterleavingQueryWithCommand()
    {
        var testRunner = fixture.Services.GetRequiredService<ITestRunner>();
        await testRunner.RunTest(
            (ICqMockBuilder mocker) => mocker,
            async (commandFactory, queryFactory) =>
            {
                var calculatorCommander = commandFactory.GetAggregate<ICalculatorAggregate>();
                // First, set the state to 10.
                await calculatorCommander
                    .Add(10)
                    .Run("testInterleavingQueryWithCommand");

                // Now, update the state without awaiting
                var commandTask = calculatorCommander
                    .Multiply(10)
                    .Delay()
                    .Add(100)
                    .Run("testInterleavingQueryWithCommand");
                
                // We delay the query as well to ensure the command is in progress.
                // The query should still return 10 as it should be timestamped
                // before the command is finished executing.
                var queryTask = queryFactory.GetAggregate<ICalculatorAggregate>()
                    .Delay()
                    .Delay()
                    .Delay()
                    .GetValue()
                    .Run("testInterleavingQueryWithCommand");

                await Task.WhenAll([commandTask, queryTask]);

                queryTask.Result.Should().Be(10);
                commandTask.Result.Should().Be(200);
            }
        );
    }

    [Fact]
    public async Task TestInterleavingQueries()
    {
        var testRunner = fixture.Services.GetRequiredService<ITestRunner>();
        await testRunner.RunTest(
            (ICqMockBuilder mocker) => mocker,
            async (commandFactory, queryFactory) =>
            {
                // First, set the state to 10.
                await commandFactory.GetAggregate<ICalculatorAggregate>()
                    .Add(10)
                    .Run("testInterleavingQueries");

                await Task.Delay(1000);

                // Now, interleave the queries.
                var queryToInterleave = queryFactory.GetAggregate<ICalculatorAggregate>()
                    .Delay()
                    .GetValue();

                var time = DateTime.UtcNow;
                var tasks = new List<Task<double>>();

                for (var i = 0; i < 100; i++)
                {
                    tasks.Add(queryToInterleave.Run("testInterleavingQueries"));
                }

                await Task.WhenAll(tasks);

                var timeNow = DateTime.UtcNow;
                var timeDiff = timeNow - time;
                // Give a slight margin of error of 1 second for how many queries are interleaving.
                // Note: if they couldn't interleave, the time difference would be at least 100 seconds.
                timeDiff.Should().BeLessThan(TimeSpan.FromSeconds(2));

                // Now, we can check the state.
                for (var i = 0; i < tasks.Count; i++)
                {
                    tasks[i].Result.Should().Be(10);
                }
            }
        );
    }

    [Fact]
    public async Task TestCommandsCantInterleave()
    {
        var testRunner = fixture.Services.GetRequiredService<ITestRunner>();
        await testRunner.RunTest(
            (ICqMockBuilder mocker) => mocker,
            async (commandFactory, queryFactory) =>
            {
                // First, set the state to 10.
                await commandFactory.GetAggregate<ICalculatorAggregate>()
                    .Add(10)
                    .Run("testCommandsCantInterleave");

                // Now, attempt interleaving the commands.
                var commandToAttemptInterleave = commandFactory.GetAggregate<ICalculatorAggregate>()
                    .Delay()
                    .Add(10);

                var time = DateTime.UtcNow;
                var tasks = new List<Task<double>>();
                var commandCount = 5;

                for (var i = 0; i < commandCount; i++)
                {
                    tasks.Add(commandToAttemptInterleave.Run("testCommandsCantInterleave"));
                    // Small delay to ensure the commands are queued in the correct order.
                    // This delay is not enough to prevent the commands from interleaving if they could.
                    await Task.Delay(1);
                }

                await Task.WhenAll(tasks);

                var timeNow = DateTime.UtcNow;
                var timeDiff = timeNow - time;
                timeDiff.Should().BeGreaterThan(TimeSpan.FromSeconds(commandCount));

                // Now, we can check the state.
                for (var i = 0; i < tasks.Count; i++)
                {
                    tasks[i].Result.Should().Be(i*10 + 20);
                }
            }
        );
    }
}