
namespace SourceGen;

[UsesVerify]
public class GenericsGenerationTests
{
    [Fact]
    public async Task GenericsGenerationTest()
    {
        // Simple test case
        const string source = @"
using Remotr;
namespace  " + nameof(GenericsGenerationTest) + @";


[UseCommand(typeof(TestCommand1Type<TestState>), ""TestC1"")]
[UseCommand(typeof(TestCommand2Type<TestState, double>), ""TestC2"")]
[UseCommand(typeof(TestCommand3Type<TestState, int, double>), ""TestC3"")]
[UseQuery(typeof(TestQuery2Type<TestState, TestState>), ""TestQ2"")]
[UseQuery(typeof(TestQuery3Type<TestState, int, double>), ""TestQ3"")]
public interface ITestAggregate : Remotr.IAggregateRoot, IGrainWithStringKey
{
}

[UseShortcuts]
public class TestCommand1Type<T> : EntityCommandHandler<T> where T : ITest
{
    public override async Task Execute()
    {
        return Task.CompletedTask;
    }
}

[UseShortcuts]
public class TestCommand2Type<T, K> : EntityCommandHandler<T, K> where K : new() where T : ITest
{
    public override async Task<double> Execute()
    {
        return new K();
    }
}

[UseShortcuts]
public class TestCommand3Type<T, K, V> : EntityCommandHandler<T, K, V> where K : new() where V : new() where T : new()
{
    public override Task<V> Execute(K input)
    {
        return new V();
    }
}

[UseShortcuts]
public class TestQuery2Type<T, K> : EntityQueryHandler<T, K> where K : new() where T : ITest
{
    public override async Task<K> Execute()
    {
        return new K();
    }
}

[UseShortcuts]
public class TestQuery3Type<T, K, V> : EntityQueryHandler<T, K, V> where K : new() where V : new() where T : ITest
{
    public override async Task<V> Execute(K input)
    {
        return new V();
    }
}

public interface ITest {}

public record TestState : ITest
{
    public double Value { get; set; }
}
";

        var driver = Utils.CreateTestDriver(source);

        // Verify the output
        await Verify(
            () => Task.FromResult(driver), 
            settings: Utils.GetVerifySettings(nameof(GenericsGenerationTest))
        );
    }

    // TODO: Test generic input, not output or class, and other variations / edge cases.
}