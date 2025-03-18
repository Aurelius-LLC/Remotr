using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Remotr.SourceGen.UseHandlerAttributes.ExtensionGenerators;
using Remotr.StatefulExtensionGenerators;
using Remotr.StatelessExtensionGenerators;

namespace Remotr.SourceGen.RemotrGenAttribute;

/// <summary>
/// Executes the code generation for classes with the RemotrGen attribute.
/// </summary>
public class RemotrExecutor
{
    private readonly ExtensionsGeneratorExecutor _statefulExtensionGenerator;
    private readonly ExtensionsGeneratorExecutor _statelessExtensionGenerator;
    private static readonly DiagnosticDescriptor InvalidTargetDiagnostic = new(
        id: "REMOTR020",
        title: "Invalid RemotrGen target",
        messageFormat: "The RemotrGen attribute can only be applied to classes that extend RootQueryHandler, RootCommandHandler, EntityQueryHandler, or EntityCommandHandler",
        category: "Remotr",
        DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    /// <summary>
    /// Initializes a new instance of the <see cref="RemotrExecutor"/> class.
    /// </summary>
    public RemotrExecutor()
    {
        _statefulExtensionGenerator = new ExtensionsGeneratorExecutor(
        [
            new EntityCommandExtensionGenerator(),
            new EntityQueryExtensionGenerator()
        ]);
        _statelessExtensionGenerator = new ExtensionsGeneratorExecutor(
        [
            new RootCommandExtensionGenerator(),
            new RootQueryExtensionGenerator()
        ]);
    }

    /// <summary>
    /// Executes the code generation for a class declaration with the RemotrGen attribute.
    /// </summary>
    /// <param name="classDeclaration">The class declaration to process</param>
    /// <param name="isValid">Whether the class is a valid target for code generation</param>
    /// <param name="context">The source production context</param>
    public void Execute(
        ClassDeclarationSyntax classDeclaration, 
        bool isValid,
        SourceProductionContext context)
    {
        if (!isValid)
        {
            context.ReportDiagnostic(Diagnostic.Create(InvalidTargetDiagnostic, classDeclaration.GetLocation()));
            return;
        }

        var baseType = classDeclaration.BaseList?.Types.FirstOrDefault()?.Type;
        if (baseType is not GenericNameSyntax genericBase)
        {
            return;
        }

        var baseTypeName = genericBase.Identifier.Text;
        
        // Process based on the handler type
        if (baseTypeName is "EntityQueryHandler" or "EntityCommandHandler")
        {
            _statefulExtensionGenerator.Generate(classDeclaration, context);
        }
        else if (baseTypeName is "RootQueryHandler" or "RootCommandHandler")
        {
            _statelessExtensionGenerator.Generate(classDeclaration, context);

        }
    }
} 