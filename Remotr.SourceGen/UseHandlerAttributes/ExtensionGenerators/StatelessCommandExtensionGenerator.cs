using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Text;
using Remotr.SourceGen.Shared;

namespace Remotr.SourceGen.UseHandlerAttributes.ExtensionGenerators;

/// <summary>
/// Generator for StatelessCommandHandler types that generates appropriate extension methods.
/// </summary>
public class StatelessCommandExtensionGenerator : BaseExtensionGenerator, IStatelessExtensionGenerator
{
    private readonly StatelessExtensionGeneratorComponent _component;
    private readonly StatelessExtensionGeneratorComponent.HandlerConfig _config;

    /// <summary>
    /// Initializes a new instance of the <see cref="StatelessCommandExtensionGenerator"/> class.
    /// </summary>
    public StatelessCommandExtensionGenerator()
    {
        _component = new StatelessExtensionGeneratorComponent();
        _config = new StatelessExtensionGeneratorComponent.HandlerConfig
        {
            BaseBuilderType = "IGrainCommandBaseBuilder",
            BuilderType = "IGrainCommandBuilder",
            HandlerType = "Command",
            OtherHandlerType = "Query",
            IncludeOtherHandler = true
        };
    }

    /// <inheritdoc/>
    protected override string HandlerBaseTypeName => "StatelessCommandHandler";

    /// <inheritdoc/>
    public override void GenerateExtensions(StringBuilder sb, string className, SeparatedSyntaxList<TypeSyntax> typeArguments)
    {
        if (!ValidateTypeArgumentCount(typeArguments, 1, 3))
            return;

        var t1 = typeArguments[0].ToString();

        switch (typeArguments.Count)
        {
            case 1:
                GenerateNoInputNoOutput(sb, className, t1);
                break;
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
        _component.GenerateNoInputNoOutput(sb, className, stateType, _config, "Tell");
    }

    /// <inheritdoc/>
    public void GenerateNoInputWithOutput(StringBuilder sb, string className, string stateType, string outputType)
    {
        _component.GenerateNoInputWithOutput(sb, className, stateType, outputType, _config, "Tell");
    }

    /// <inheritdoc/>
    public void GenerateWithInputAndOutput(StringBuilder sb, string className, string stateType, string inputType, string outputType)
    {
        _component.GenerateWithInputAndOutput(sb, className, stateType, inputType, outputType, _config, "Tell", "ThenTell");
    }
} 