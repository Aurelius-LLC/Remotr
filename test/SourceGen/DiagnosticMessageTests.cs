namespace SourceGen;

[UsesVerify]
public class DiagnosticMessageTests
{
    [Fact]
    public async Task MissingIAggregateRootInterfaceTest()
    {
        const string source = @"
using Remotr;
namespace " + nameof(MissingIAggregateRootInterfaceTest) + @";


[UseCommand(typeof(TestCommand1Type), ""TestC1"")]
[UseQuery(typeof(TestQuery2Type), ""TestQ2"")]
public interface ITestAggregate : IGrain, IGrainWithStringKey
{
}

public class TestCommand1Type : StatefulCommandHandler<TestState>
{
    public override Task Execute()
    {
        return Task.CompletedTask;
    }
}

public class TestQuery2Type : StatefulQueryHandler<TestState, double>
{
    public override async Task<double> Execute()
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
            settings: Utils.GetVerifySettings(nameof(MissingIAggregateRootInterfaceTest))
        );
    }


    [Fact]
    public async Task InvalidKeyStrategyDelegateNumberOfParametersTest()
    {
        const string source = @"
using Remotr;
namespace " + nameof(InvalidKeyStrategyDelegateNumberOfParametersTest) + @";


[UseCommand(typeof(TestCommand3Type), ""TestC3"", findMethod: nameof(SetValue3TypeKey))]
[UseQuery(typeof(TestQuery2Type), ""TestQ2"", findMethod: nameof(SetValue2TypeKey))]
public interface ITestAggregate : IAggregateRoot, IGrainWithStringKey
{
    public static string SetValue2TypeKey(double input) {
        return ""testKey2"";
    }

    public static string SetValue3TypeKey() {
        return ""testKey3"";
    }
}

public class TestCommand3Type : StatefulCommandHandler<TestState, double, double>
{
    public override Task<double> Execute(double input)
    {
        return 0.0;
    }
}

public class TestQuery2Type : StatefulQueryHandler<TestState, double>
{
    public override async Task<double> Execute()
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
            settings: Utils.GetVerifySettings(nameof(InvalidKeyStrategyDelegateNumberOfParametersTest))
        );
    }

    [Fact]
    public async Task InvalidKeyStrategyDelegateInputTypeTest()
    {
        const string source = @"
using Remotr;
namespace " + nameof(InvalidKeyStrategyDelegateInputTypeTest) + @";


[UseCommand(typeof(TestCommand3Type), ""TestC3"", findMethod: nameof(SetValue3TypeKey))]
public interface ITestAggregate : IAggregateRoot, IGrainWithStringKey
{
    public static string SetValue3TypeKey(string input) {
        return ""testKey3"";
    }
}

public class TestCommand3Type : StatefulCommandHandler<TestState, double, double>
{
    public override Task<double> Execute(double input)
    {
        return Task.FromResult(0.0);
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
            settings: Utils.GetVerifySettings(nameof(InvalidKeyStrategyDelegateInputTypeTest))
        );
    }

    [Fact]
    public async Task InvalidKeyStrategyDelegateReturnTypeTest()
    {
        const string source = @"
using Remotr;
namespace " + nameof(InvalidKeyStrategyDelegateReturnTypeTest) + @";


[UseCommand(typeof(TestCommand3Type), ""TestC3"", findMethod: nameof(SetValue3TypeKey))]
public interface ITestAggregate : IAggregateRoot, IGrainWithStringKey
{
    public static int SetValue3TypeKey(double input) {
        return 123;
    }
}

public class TestCommand3Type : StatefulCommandHandler<TestState, double, double>
{
    public override Task<double> Execute(double input)
    {
        return Task.FromResult(0.0);
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
            settings: Utils.GetVerifySettings(nameof(InvalidKeyStrategyDelegateReturnTypeTest))
        );
    }

    [Fact]
    public async Task InvalidKeyStrategyDelegateNameTest()
    {
        const string source = @"
using Remotr;
namespace " + nameof(InvalidKeyStrategyDelegateNameTest) + @";


[UseCommand(typeof(TestCommand3Type), ""TestC3"", findMethod: ""NonExistentMethod"")]
public interface ITestAggregate : IAggregateRoot, IGrainWithStringKey
{
    public static string SetValue3TypeKey(double input) {
        return ""testKey3"";
    }
}

public class TestCommand3Type : StatefulCommandHandler<TestState, double, double>
{
    public override Task<double> Execute(double input)
    {
        return Task.FromResult(0.0);
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
            settings: Utils.GetVerifySettings(nameof(InvalidKeyStrategyDelegateNameTest))
        );
    }

    [Fact]
    public async Task UseCommandInvalidHandlerTypeTest()
    {
        const string source = @"
using Remotr;
namespace " + nameof(UseCommandInvalidHandlerTypeTest) + @";


[UseCommand(typeof(TestQueryType), ""TestC"")]
public interface ITestAggregate : IAggregateRoot, IGrainWithStringKey
{
}

public class TestQueryType : StatefulQueryHandler<TestState, double>
{
    public override Task<double> Execute()
    {
        return Task.FromResult(0.0);
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
            settings: Utils.GetVerifySettings(nameof(UseCommandInvalidHandlerTypeTest))
        );
    }

    [Fact]
    public async Task UseQueryInvalidHandlerTypeTest()
    {
        const string source = @"
using Remotr;
namespace " + nameof(UseQueryInvalidHandlerTypeTest) + @";


[UseQuery(typeof(TestCommandType), ""TestQ"")]
public interface ITestAggregate : IAggregateRoot, IGrainWithStringKey
{
}

public class TestCommandType : StatefulCommandHandler<TestState, double>
{
    public override Task<double> Execute()
    {
        return Task.FromResult(0.0);
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
            settings: Utils.GetVerifySettings(nameof(UseQueryInvalidHandlerTypeTest))
        );
    }

    [Fact]
    public async Task RemotrGenAttributeOnInvalidTargetTest()
    {
        const string source = @"
using Remotr;
namespace " + nameof(RemotrGenAttributeOnInvalidTargetTest) + @";

public interface ITestAggregate : IAggregateRoot, IGrainWithStringKey
{
}

[RemotrGen]
public class TestCommandType
{
    public Task<double> Execute()
    {
        return Task.FromResult(0.0);
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
            settings: Utils.GetVerifySettings(nameof(RemotrGenAttributeOnInvalidTargetTest))
        );
    }
}