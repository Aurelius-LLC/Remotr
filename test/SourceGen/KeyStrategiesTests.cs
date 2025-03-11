using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Remotr;
using Remotr.SourceGen;
using Remotr.SourceGen.HandlerAttributes;
using System;
using System.Collections.Immutable;
using System.Reflection;
using System.Threading.Tasks;
using VerifyXunit;

namespace SourceGen;

[UsesVerify]
public class KeyStrategiesTests
{



    [Fact]
    public async Task KeyStrategiesGenerationTest()
    {
        string source = @"
using Remotr;
namespace " + nameof(KeyStrategiesGenerationTest) + @";

[UseCommand(typeof(TestCommand2Type), ""TestC2"", fixedKey: ""testKey1"")]
[UseCommand(typeof(TestCommand3Type), ""TestC3"", findMethod: nameof(SetValue3TypeKey))]
[UseQuery(typeof(TestQuery2Type), ""TestQ2"", findMethod: nameof(SetValue2TypeKey))]
[UseQuery(typeof(TestQuery3Type), ""TestQ3"", fixedKey: ""testKey2"")]
public interface ITestManagerGrain : Remotr.ITransactionManagerGrain, IGrainWithStringKey
{
    public static string SetValue2TypeKey() {
        return ""testKey2"";
    }

    public static string SetValue3TypeKey(TestInputObject input) {
        return ""testKey1"";
    }
}

[RemotrGen]
public class TestCommand2Type : StatefulCommandHandler<TestState, double>
{
    public override async Task<double> Execute()
    {
        return (await GetState()).Value;
    }
}

[RemotrGen]
public class TestCommand3Type : StatefulCommandHandler<TestState, TestInputObject, double>
{
    public override async Task<double> Execute(TestInputObject input)
    {
        return (await GetState()).Value;
    }
}

[RemotrGen]
public class TestQuery2Type : StatefulQueryHandler<TestState, double>
{
    public override async Task<double> Execute()
    {
        return (await GetState()).Value;
    }
}

[RemotrGen]
public class TestQuery3Type : StatefulQueryHandler<TestState, double, double>
{
    public override async Task<double> Execute(double input)
    {
        return (await GetState()).Value;
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
            settings: Utils.GetVerifySettings(nameof(KeyStrategiesGenerationTest))
        );
    }
}

