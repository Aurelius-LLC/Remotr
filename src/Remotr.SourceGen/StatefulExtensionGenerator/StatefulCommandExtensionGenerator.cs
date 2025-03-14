using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Text;
using Remotr.SourceGen.Shared;

namespace Remotr.StatefulExtensionGenerators;

/// <summary>
/// Generator for StatefulCommandHandler types that generates appropriate extension methods.
/// </summary>
public class StatefulCommandExtensionGenerator : BaseExtensionGenerator, IStatefulExtensionGenerator
{
    private readonly StatefulExtensionGeneratorComponent _component;
    private readonly ExtensionConfig _config;

    /// <summary>
    /// Initializes a new instance of the <see cref="StatefulCommandExtensionGenerator"/> class.
    /// </summary>
    public StatefulCommandExtensionGenerator()
    {
        _component = new StatefulExtensionGeneratorComponent();
        _config = new ExtensionConfig
        {
            BaseBuilderType = "IGrainCommandBaseBuilder",
            BuilderType = "IGrainCommandBuilder",
            HandlerType = "Command",
            OtherHandlerType = "Query",
            IncludeOtherHandler = true
        };
    }

    /// <inheritdoc/>
    protected override string HandlerBaseTypeName => "StatefulCommandHandler";

    /// <inheritdoc/>
    public override void GenerateExtensions(
        StringBuilder sb, 
        string className, 
        SeparatedSyntaxList<TypeSyntax> typeArguments,
        string implementationGenericTypes,
        string implementationGenericTypesWithOther,
        string genericConstraints,
        string otherGenericType)
    {
        if (!ValidateTypeArgumentCount(typeArguments, 1, 3))
            return;

        var t1 = typeArguments[0].ToString();

        switch (typeArguments.Count)
        {
            case 1:
                GenerateNoInputNoOutput(sb, className, t1, implementationGenericTypes, implementationGenericTypesWithOther, genericConstraints, otherGenericType);
                break;
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
        string otherGenericType)
    {
        _component.GenerateNoInputNoOutput(sb, className, stateType, _config, "Tell", implementationGenericTypes, implementationGenericTypesWithOther, genericConstraints, otherGenericType);
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
        _component.GenerateNoInputWithOutput(sb, className, stateType, outputType, _config, "Tell", implementationGenericTypes, implementationGenericTypesWithOther, genericConstraints, otherGenericType);
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
        _component.GenerateWithInputAndOutput(sb, className, stateType, inputType, outputType, _config, "Tell", "ThenTell", implementationGenericTypes, implementationGenericTypesWithOther, genericConstraints, otherGenericType);
    }
} 