using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Remotr;
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
    private static readonly VerifySettings Settings;
    
    static SourceGeneratorTests()
    {
        Settings = new VerifySettings();
        Settings.UseDirectory("snapshots");
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

    [Fact]
    public async Task BasicTest()
    {
        // Simple test case
        const string source = @"
namespace Test1;

[Remotr.UseQuery(typeof(GetValueState3Type), ""GetValue3Type"", findMethod: nameof(GetValueState3TypeKey))]
public interface ICalculatorManagerGrain : Remotr.ITransactionManagerGrain, IGrainWithStringKey
{
    public static string GetValueState3TypeKey(int input) 
    {
        return ""Calculator"";
    }
}
public class GetValueState3Type : StatefulQueryHandler<CalculatorState, int, double>
{
    public override async Task<double> Execute(int input)
    {
        return (await GetState()).Value;
    }
}
public record CalculatorState
{
    public double Value { get; set; }
}
";

        // Parse the provided string into a C# syntax tree
        SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(source);

        // Create a Roslyn compilation for the syntax tree
        var references = GetMetadataReferences();

        CSharpCompilation compilation = CSharpCompilation.Create(
            assemblyName: "Tests",
            syntaxTrees: new[] { syntaxTree },
            references: references,
            options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

        // Create an instance of our generator
        var generator = new HandlerAttributeIncrementalGenerator();
        
        // Create the driver that will run our generator
        GeneratorDriver driver = CSharpGeneratorDriver.Create(generator);
        
        // Run the source generator
        driver = driver.RunGenerators(compilation);

        // Verify the output
        await Verify(() => Task.FromResult(driver), settings: Settings);
    }
}

