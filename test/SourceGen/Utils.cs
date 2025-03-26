
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Remotr;
using Remotr.SourceGen;
using Remotr.SourceGen.UseHandlerAttributes;
using System.Reflection;

namespace SourceGen;

public static class Utils {
    public static VerifySettings GetVerifySettings(string testName)
    {
        VerifySettings settings = new();
        settings.UseDirectory("snapshots/" + testName);
        return settings;
    }

    public static IEnumerable<MetadataReference> GetMetadataReferences()
    {
        var assemblies = new[]
        {
            typeof(object).Assembly, // System.Private.CoreLib
            typeof(IAggregateRoot).Assembly, // Remotr
            Assembly.Load("netstandard"),
            Assembly.Load("System.Runtime"),
        };

        return assemblies.Select(assembly => MetadataReference.CreateFromFile(assembly.Location));
    }

    public static GeneratorDriver CreateTestDriver(string source)
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
        var UseShortcutserator = Generators.GetUseShortcutserator();
        
        // Create the driver that will run our generators
        GeneratorDriver driver = CSharpGeneratorDriver.Create([handlerGenerator, UseShortcutserator]);
        
        // Run the source generators
        driver = driver.RunGenerators(compilation);

        return driver;
    }
}