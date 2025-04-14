using System.Collections.Immutable;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Remotr;
using Remotr.Testing;
using Remotr.Samples.Calculator;
using Remotr.Samples.Calendar;

namespace RemotrTests.Tests;

[Collection(ClusterCollection.Name)]
public class CqrsChainingTests(ClusterFixture fixture)
{
    [Fact]
    public async Task ChainingTest()
    {
        var testRunner = fixture.Services.GetRequiredService<ITestRunner>();
        await testRunner.RunTest(
            (ICqMockBuilder mocker) => mocker,
            async (commandFactory, queryFactory) =>
            {
                SimpleExamples simpleExamples = new SimpleExamples(commandFactory, queryFactory);
                var simpleArithmeticTest = await simpleExamples.SimpleArithmetic("simpleArithmetic"); 
                var cachingChainsTest = await simpleExamples.CachingChains("target1", "target2");
                var advancedChainingTest = await simpleExamples.AdvancedChaining("advancedChaining");
                var advancedChaining2Test = await simpleExamples.AdvancedChaining2("advancedChaining2");

                // The value from the advanced chaining test should be different than what the state is.
                // This checks what the state's value is.
                var valueFromAdvancedChainingTest = await queryFactory.GetAggregate<ICalculatorAggregate>()
                    .GetValue()
                    .Run("advancedChaining");

                // The value from the advanced chaining test 2 should be different than what the state is.
                // This checks what the state's value is.
                var valueFromAdvancedChainingTest2 = await queryFactory.GetAggregate<ICalculatorAggregate>()
                    .GetValue()
                    .Run("advancedChaining2");

                simpleArithmeticTest.Should().Be(4.0);
                cachingChainsTest.Should().Be((1.0, 1.0, 1.5));
                advancedChainingTest.Should().Be(15800.0);
                valueFromAdvancedChainingTest.Should().Be(12000.0);

                advancedChaining2Test.ToList().Should().BeEquivalentTo([11, 26, 87]);
                valueFromAdvancedChainingTest2.Should().Be(87.0);
            }
        );
    }

    [Fact]
    public async Task StepsShouldIsolateOutputsTest() {
        var testRunner = fixture.Services.GetRequiredService<ITestRunner>();
        await testRunner.RunTest(
            (ICqMockBuilder mocker) => mocker,
            async (commandFactory, queryFactory) =>
            {
                var result = await commandFactory.GetAggregate<ICalendarAggregate>()
                    .AddEvent(new EventState () {
                        Title = "Test",
                        Duration = new (2),
                    })
                    .MergeSplit(
                        // This updates the input.
                        (b1) => b1.ThenUpdateEventStateInput(),
                        // But this is the value we are taking, which should NOT have the updated input.
                        (b2) => b2,
                        new TakeSecond<EventState, EventState>()
                    )
                    .Run("StepsShouldIsolateOutputsTest");
                
                result.Duration.Should().Be(new TimeOnly(2));
            }
        );
    }
}
