

namespace SourceGen;

[UsesVerify]
public class BasicGenerationTests
{
    [Fact]
    public async Task SimpleCommandsAndQueriesGenerationTest()
    {
        // Simple test case
        const string source = @"
using Remotr;
namespace SimpleCommandsAndQueriesTest;


[UseCommand(typeof(TestCommand1Type), ""TestC1"")]
[UseCommand(typeof(TestCommand2Type), ""TestC2"")]
[UseCommand(typeof(TestCommand3Type), ""TestC3"")]
[UseQuery(typeof(TestQuery2Type), ""TestQ2"")]
[UseQuery(typeof(TestQuery3Type), ""TestQ3"")]
public interface ITestAggregate : Remotr.IAggregateRoot, IGrainWithStringKey
{
}

[UseShortcuts]
public class TestCommand1Type : EntityCommandHandler<TestState>
{
    public override Task Execute()
    {
        return Task.CompletedTask;
    }
}

[UseShortcuts]
public class TestCommand2Type : EntityCommandHandler<TestState, double>
{
    public override async Task<double> Execute()
    {
        return (await GetState()).Value;
    }
}

[UseShortcuts]
public class TestCommand3Type : EntityCommandHandler<TestState, int, double>
{
    public override async Task<double> Execute(int input)
    {
        return (await GetState()).Value;
    }
}

[UseShortcuts]
public class TestQuery2Type : EntityQueryHandler<TestState, double>
{
    public override async Task<double> Execute()
    {
        return (await GetState()).Value;
    }
}

[UseShortcuts]
public class TestQuery3Type : EntityQueryHandler<TestState, int, double>
{
    public override async Task<double> Execute(int input)
    {
        return (await GetState()).Value;
    }
}


public record TestState
{
    public double Value { get; set; }
}
";

        var driver = Utils.CreateTestDriver(source);

        // Verify the output
        await Verify(
            () => Task.FromResult(driver), 
            settings: Utils.GetVerifySettings(nameof(SimpleCommandsAndQueriesGenerationTest))
        );
    }


    [Fact]
    public async Task CommandsAndQueriesWithObjectsGenerationTest()
    {
        string source = @"
using Remotr;
namespace  " + nameof(CommandsAndQueriesWithObjectsGenerationTest) + @";

[UseCommand(typeof(TestCommand3Type), ""TestC3"")]
[UseQuery(typeof(TestQuery3Type), ""TestQ3"")]
public interface ITestAggregate : Remotr.IAggregateRoot, IGrainWithStringKey
{
}

[UseShortcuts]
public class TestCommand3Type : EntityCommandHandler<TestState, TestInputObject, double>
{
    public override async Task<double> Execute(TestInputObject input)
    {
        return (await GetState()).Value;
    }
}

[UseShortcuts]
public class TestQuery3Type : EntityQueryHandler<TestState, TestInputObject, TestOutputObject>
{
    public override async Task<TestOutputObject> Execute(TestInputObject input)
    {
        return new TestOutputObject();
    }
}


public record TestState
{
    public double Value { get; set; }
}

public record TestInputObject
{
    public int Value1 { get; set; }
    public double Value2 { get; set; }
}

public record TestOutputObject
{
    public string Value1 { get; set; }
    public string Value2 { get; set; }
}
";

        var driver = Utils.CreateTestDriver(source);

        // Verify the output
        await Verify(
            () => Task.FromResult(driver), 
            settings: Utils.GetVerifySettings(nameof(CommandsAndQueriesWithObjectsGenerationTest))
        );
    }

    [Fact]
    public async Task StatelessHandlerExtensionGenerationTests()
    {
        string source = @"
using Remotr;
namespace  " + nameof(StatelessHandlerExtensionGenerationTests) + @";

public interface ITestAggregate : Remotr.IAggregateRoot, IGrainWithStringKey
{
}

[UseShortcuts]
public class TestCommand1Type : RootCommandHandler<ITestAggregate, TestInputObject, double>
{
    public override async Task<double> Execute(TestInputObject input)
    {
        return 0.0;
    }
}

[UseShortcuts]
public class TestQuery3Type : RootQueryHandler<ITestAggregate, TestInputObject, TestOutputObject>
{
    public override async Task<TestOutputObject> Execute(TestInputObject input)
    {
        return new TestOutputObject();
    }
}


public record TestState
{
    public double Value { get; set; }
}

public record TestInputObject
{
    public int Value1 { get; set; }
    public double Value2 { get; set; }
}

public record TestOutputObject
{
    public string Value1 { get; set; }
    public string Value2 { get; set; }
}
";

        var driver = Utils.CreateTestDriver(source);

        // Verify the output
        await Verify(
            () => Task.FromResult(driver), 
            settings: Utils.GetVerifySettings(nameof(StatelessHandlerExtensionGenerationTests))
        );
    }
}