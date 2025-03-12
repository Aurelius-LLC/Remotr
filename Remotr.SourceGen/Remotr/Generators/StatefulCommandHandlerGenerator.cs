using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Text;
using Remotr.SourceGen.Shared;

namespace Remotr.SourceGen.Remotr;

/// <summary>
/// Generator for StatefulCommandHandler types that generates appropriate extension methods.
/// </summary>
public class StatefulCommandHandlerGenerator : BaseExtensionGenerator, IStatefulHandlerGenerator
{
    private readonly StatefulHandlerGeneratorComponent _component;

    /// <summary>
    /// Initializes a new instance of the <see cref="StatefulCommandHandlerGenerator"/> class.
    /// </summary>
    public StatefulCommandHandlerGenerator()
    {
        _component = new StatefulHandlerGeneratorComponent();
    }

    /// <inheritdoc/>
    protected override string HandlerBaseTypeName => "StatefulCommandHandler";

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
        _component.GenerateNoInputNoOutput(sb, className, stateType, "Command");
    }

    /// <inheritdoc/>
    public void GenerateNoInputWithOutput(StringBuilder sb, string className, string stateType, string outputType)
    {
        _component.GenerateNoInputWithOutput(sb, className, stateType, outputType, "Command", "Tell");
    }

    /// <inheritdoc/>
    public void GenerateWithInputAndOutput(StringBuilder sb, string className, string stateType, string inputType, string outputType)
    {
        _component.GenerateWithInputAndOutput(sb, className, stateType, inputType, outputType, "Command", "Tell", "ThenTell");
    }
} 