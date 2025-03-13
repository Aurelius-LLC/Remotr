using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Text;
using Remotr.SourceGen.Shared;

namespace Remotr.SourceGen.Remotr;

/// <summary>
/// Generator for StatefulQueryHandler types that generates appropriate extension methods.
/// </summary>
public class StatefulQueryHandlerGenerator : BaseExtensionGenerator, IStatefulHandlerGenerator
{
    private readonly StatefulHandlerGeneratorComponent _component;
    private readonly StatefulHandlerGeneratorComponent.HandlerConfig _queryConfig;
    private readonly StatefulHandlerGeneratorComponent.HandlerConfig _commandToQueryConfig;

    /// <summary>
    /// Initializes a new instance of the <see cref="StatefulQueryHandlerGenerator"/> class.
    /// </summary>
    public StatefulQueryHandlerGenerator()
    {
        _component = new StatefulHandlerGeneratorComponent();
        
        // Configuration for query-specific extensions
        _queryConfig = new StatefulHandlerGeneratorComponent.HandlerConfig
        {
            BaseBuilderType = "IGrainQueryBaseBuilder",
            BuilderType = "IGrainQueryBuilder",
            HandlerType = "Query",
            IncludeOtherHandler = false
        };

        // Configuration for command-to-query extensions
        _commandToQueryConfig = new StatefulHandlerGeneratorComponent.HandlerConfig
        {
            BaseBuilderType = "IGrainCommandBaseBuilder",
            BuilderType = "IGrainCommandBuilder",
            HandlerType = "Command",
            OtherHandlerType = "Query",
            IncludeOtherHandler = true
        };
    }

    /// <inheritdoc/>
    protected override string HandlerBaseTypeName => "StatefulQueryHandler";

    /// <inheritdoc/>
    public override void GenerateExtensions(StringBuilder sb, string className, SeparatedSyntaxList<TypeSyntax> typeArguments)
    {
        if (!ValidateTypeArgumentCount(typeArguments, 2, 3))
            return;

        var t1 = typeArguments[0].ToString();

        switch (typeArguments.Count)
        {
            case 2:
                GenerateNoInput(sb, className, t1, typeArguments[1].ToString());
                break;
            case 3:
                GenerateWithInput(sb, className, t1, typeArguments[1].ToString(), typeArguments[2].ToString());
                break;
        }
    }

    /// <inheritdoc/>
    public void GenerateNoInputNoOutput(StringBuilder sb, string className, string stateType)
    {
        throw new NotImplementedException("Query handlers must have at least an output.");
    }

    /// <inheritdoc/>
    public void GenerateNoInputWithOutput(StringBuilder sb, string className, string stateType, string outputType)
    {
        GenerateNoInput(sb, className, stateType, outputType);
    }

    /// <inheritdoc/>
    public void GenerateWithInputAndOutput(StringBuilder sb, string className, string stateType, string inputType, string outputType)
    {
        GenerateWithInput(sb, className, stateType, inputType, outputType);
    }

    private void GenerateNoInput(StringBuilder sb, string className, string stateType, string outputType)
    {
        // Generate query-specific extension
        _component.GenerateNoInputWithOutput(sb, className, stateType, outputType, _queryConfig, "Ask");
        
        sb.AppendLine();

        // Generate command-to-query extension
        _component.GenerateNoInputWithOutput(sb, className, stateType, outputType, _commandToQueryConfig, "Ask");
    }

    private void GenerateWithInput(StringBuilder sb, string className, string stateType, string inputType, string outputType)
    {
        // Generate query-specific extension
        _component.GenerateWithInputAndOutput(sb, className, stateType, inputType, outputType, _queryConfig, "Ask", "ThenAsk");

        sb.AppendLine();
        
        // Generate command-to-query extension
        _component.GenerateWithInputAndOutput(sb, className, stateType, inputType, outputType, _commandToQueryConfig, "Ask", "ThenAsk");
    }
} 