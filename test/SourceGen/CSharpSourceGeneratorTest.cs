using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using VerifyXunit;

namespace SourceGen;

// Helper class for verifying source generators with snapshot testing
public static class CSharpVerifier
{
    public static Task Verify<TGenerator>(string source) 
        where TGenerator : class, new()
    {
        // Parse the provided string into a C# syntax tree
        SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(source);

        // Create a Roslyn compilation for the syntax tree
        var references = new[]
        {
            MetadataReference.CreateFromFile(typeof(object).Assembly.Location)
        };

        CSharpCompilation compilation = CSharpCompilation.Create(
            assemblyName: "Tests",
            syntaxTrees: new[] { syntaxTree },
            references: references,
            options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

        // Create an instance of our generator
        var generator = new TGenerator();
        
        // Create the driver that will run our generator
        GeneratorDriver driver;
        
        if (generator is IIncrementalGenerator incrementalGenerator)
        {
            // For IIncrementalGenerator, we need to create a GeneratorDriver directly
            driver = CSharpGeneratorDriver.Create(
                new ISourceGenerator[] { new IncrementalGeneratorWrapper(incrementalGenerator) });
        }
        else if (generator is ISourceGenerator sourceGenerator)
        {
            driver = CSharpGeneratorDriver.Create(
                generators: new[] { sourceGenerator });
        }
        else
        {
            throw new ArgumentException(
                $"Generator must implement either IIncrementalGenerator or ISourceGenerator", 
                nameof(TGenerator));
        }

        // Run the source generator
        driver = driver.RunGenerators(compilation);

        // Use Verify to snapshot test the source generator output
        return Verifier.Verify(driver);
    }
    
    // Wrapper class to adapt IIncrementalGenerator to ISourceGenerator
    public class IncrementalGeneratorWrapper : ISourceGenerator
    {
        private readonly IIncrementalGenerator _incrementalGenerator;

        public IncrementalGeneratorWrapper(IIncrementalGenerator incrementalGenerator)
        {
            _incrementalGenerator = incrementalGenerator;
        }

        public void Execute(GeneratorExecutionContext context)
        {
            // This won't be called when using the generator driver
        }

        public void Initialize(GeneratorInitializationContext context)
        {
            // This won't be called when using the generator driver
        }
    }
}