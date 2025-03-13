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
    private readonly StatelessExtensionGeneratorComponent.HandlerConfig _queryBuilderConfig;
    private readonly StatelessExtensionGeneratorComponent.HandlerConfig _commandBuilderConfig;

    /// <summary>
    /// Initializes a new instance of the <see cref="StatelessQueryExtensionGenerator"/> class.
    /// </summary>
    public StatelessQueryExtensionGenerator()
    {
        _component = new StatelessExtensionGeneratorComponent();
        _queryBuilderConfig = new StatelessExtensionGeneratorComponent.HandlerConfig
        {
            BaseBuilderType = "IGrainQueryBaseBuilder",
            BuilderType = "IGrainQueryBuilder",
            HandlerType = "Query",
            OtherHandlerType = "",
            IncludeOtherHandler = false
        };

        _commandBuilderConfig = new StatelessExtensionGeneratorComponent.HandlerConfig
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
    public override void GenerateExtensions(StringBuilder sb, string className, SeparatedSyntaxList<TypeSyntax> typeArguments)
    {
        if (!ValidateTypeArgumentCount(typeArguments, 2, 3))
            return;

        var t1 = typeArguments[0].ToString();

        switch (typeArguments.Count)
        {
            case 2:
                GenerateNoInputWithOutput(sb, className, t1, typeArguments[1].ToString());
                break;
            case 3:
                GenerateWithInputAndOutput(sb, className, t1, typeArguments[1].ToString(), typeArguments[2].ToString());
                break;
        }
    }

    /// <inheritdoc/>
    public void GenerateNoInputNoOutput(StringBuilder sb, string className, string stateType)
    {
        throw new System.NotImplementedException("Query handlers must have at least an output.");
    }

    /// <inheritdoc/>
    public void GenerateNoInputWithOutput(StringBuilder sb, string className, string stateType, string outputType)
    {
        // Generate query-specific extension
        _component.GenerateNoInputWithOutput(sb, className, stateType, outputType, _queryBuilderConfig, "Ask");
        
        sb.AppendLine();

        // Generate command-to-query extension
        _component.GenerateNoInputWithOutput(sb, className, stateType, outputType, _commandBuilderConfig, "Ask");
        
    }

    /// <inheritdoc/>
    public void GenerateWithInputAndOutput(StringBuilder sb, string className, string stateType, string inputType, string outputType)
    {
        _component.GenerateWithInputAndOutput(sb, className, stateType, inputType, outputType, _queryBuilderConfig, "Ask", "ThenAsk");

        sb.AppendLine();

        _component.GenerateWithInputAndOutput(sb, className, stateType, inputType, outputType, _commandBuilderConfig, "Ask", "ThenAsk");
    }
} 