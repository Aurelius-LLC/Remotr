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
public class SourceGeneratorTests
{
    private static VerifySettings GetVerifySettings(string testName)
    {
        VerifySettings settings = new();
        settings.UseDirectory("snapshots/" + testName);
        return settings;
    }

    private static IEnumerable<MetadataReference> GetMetadataReferences()
    {
        var assemblies = new[]
        {
            typeof(object).Assembly, // System.Private.CoreLib
            typeof(ITransactionManagerGrain).Assembly, // Remotr
            Assembly.Load("netstandard"),
            Assembly.Load("System.Runtime"),
        };

        return assemblies.Select(assembly => MetadataReference.CreateFromFile(assembly.Location));
    }

    private static GeneratorDriver CreateTestDriver(string source)
    {
        // Parse the provided string into a C# syntax tree
        SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(source);

        // Create a Roslyn compilation for the syntax tree
        var references = GetMetadataReferences();

        CSharpCompilation compilation = CSharpCompilation.Create(
            assemblyName: "Tests",
            syntaxTrees: new[] { syntaxTree },
            references: references,
            options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

        // Create instances of our generators
        var handlerGenerator = new HandlerAttributeIncrementalGenerator();
        var remotrGenerator = Generators.GetRemotrGenerator();
        
        // Create the driver that will run our generators
        GeneratorDriver driver = CSharpGeneratorDriver.Create([handlerGenerator, remotrGenerator]);
        
        // Run the source generators
        driver = driver.RunGenerators(compilation);

        return driver;
    }

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
public interface ITestManagerGrain : Remotr.ITransactionManagerGrain, IGrainWithStringKey
{
}

[RemotrGen]
public class TestCommand1Type : StatefulCommandHandler<TestState>
{
    public override Task Execute()
    {
        return Task.CompletedTask;
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
public class TestCommand3Type : StatefulCommandHandler<TestState, int, double>
{
    public override async Task<double> Execute(int input)
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
public class TestQuery3Type : StatefulQueryHandler<TestState, int, double>
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

        var driver = CreateTestDriver(source);

        // Verify the output
        await Verify(() => Task.FromResult(driver), settings: GetVerifySettings(nameof(SimpleCommandsAndQueriesGenerationTest)));
    }
}

