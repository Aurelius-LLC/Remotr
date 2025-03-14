using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Text;
using Remotr.SourceGen.Shared;

namespace Remotr.StatelessExtensionGenerators;

/// <summary>
/// Generator for StatelessQueryHandler types that generates appropriate extension methods.
/// </summary>
public class StatelessQueryExtensionGenerator : BaseExtensionGenerator, IStatelessExtensionGenerator
{
    private readonly StatelessExtensionGeneratorComponent _component;
    private readonly ExtensionConfig _queryBuilderConfig;
    private readonly ExtensionConfig _commandBuilderConfig;

    /// <summary>
    /// Initializes a new instance of the <see cref="StatelessQueryExtensionGenerator"/> class.
    /// </summary>
    public StatelessQueryExtensionGenerator()
    {
        _component = new StatelessExtensionGeneratorComponent();
        _queryBuilderConfig = new ExtensionConfig
        {
            BaseBuilderType = "IGrainQueryBaseBuilder",
            BuilderType = "IGrainQueryBuilder",
            HandlerType = "Query",
            OtherHandlerType = "",
            IncludeOtherHandler = false
        };

        _commandBuilderConfig = new ExtensionConfig
        {
            BaseBuilderType = "IGrainCommandBaseBuilder",
            BuilderType = "IGrainCommandBuilder",
            HandlerType = "Command",
            OtherHandlerType = "Query",
            IncludeOtherHandler = true
        };
    }

    /// <inheritdoc/>
    protected override string HandlerBaseTypeName => "StatelessQueryHandler";

    /// <inheritdoc/>
    public override void GenerateExtensions(
        StringBuilder sb, 
        string className, 
        SeparatedSyntaxList<TypeSyntax> typeArguments,
        string implementationGenericTypes = "",
        string implementationGenericTypesWithOther = "<T>",
        string genericConstraints = "",
        string otherGenericType = "T")
    {
        if (!ValidateTypeArgumentCount(typeArguments, 2, 3))
            return;

        var t1 = typeArguments[0].ToString();

        switch (typeArguments.Count)
        {
            case 2:
                GenerateNoInputWithOutput(sb, className, t1, typeArguments[1].ToString(), implementationGenericTypes, implementationGenericTypesWithOther, genericConstraints, otherGenericType);
                break;
            case 3:
                GenerateWithInputAndOutput(sb, className, t1, typeArguments[1].ToString(), typeArguments[2].ToString(), implementationGenericTypes, implementationGenericTypesWithOther, genericConstraints, otherGenericType);
                break;
        }
    }

    /// <inheritdoc/>
    public void GenerateNoInputNoOutput(
        StringBuilder sb, 
        string className, 
        string stateType,
        string implementationGenericTypes,
        string implementationGenericTypesWithOther,
        string genericConstraints,
        string otherGenericType = "T")
    {
        throw new System.NotImplementedException("Query handlers must have at least an output.");
    }

    /// <inheritdoc/>
    public void GenerateNoInputWithOutput(
        StringBuilder sb, 
        string className, 
        string stateType, 
        string outputType,
        string implementationGenericTypes,
        string implementationGenericTypesWithOther,
        string genericConstraints,
        string otherGenericType)
    {
        // Generate query-specific extension
        _component.GenerateNoInputWithOutput(sb, className, stateType, outputType, _queryBuilderConfig, "Ask", implementationGenericTypes, implementationGenericTypesWithOther, genericConstraints, otherGenericType);
        
        sb.AppendLine();

        // Generate command-to-query extension
        _component.GenerateNoInputWithOutput(sb, className, stateType, outputType, _commandBuilderConfig, "Ask", implementationGenericTypes, implementationGenericTypesWithOther, genericConstraints, otherGenericType);
        
    }

    /// <inheritdoc/>
    public void GenerateWithInputAndOutput(
        StringBuilder sb, 
        string className, 
        string stateType, 
        string inputType, 
        string outputType,
        string implementationGenericTypes,
        string implementationGenericTypesWithOther,
        string genericConstraints,
        string otherGenericType)
    {
        _component.GenerateWithInputAndOutput(sb, className, stateType, inputType, outputType, _queryBuilderConfig, "Ask", "ThenAsk", implementationGenericTypes, implementationGenericTypesWithOther, genericConstraints, otherGenericType);

        sb.AppendLine();

        _component.GenerateWithInputAndOutput(sb, className, stateType, inputType, outputType, _commandBuilderConfig, "Ask", "ThenAsk", implementationGenericTypes, implementationGenericTypesWithOther, genericConstraints, otherGenericType);
    }
} 