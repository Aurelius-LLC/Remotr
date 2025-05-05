using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using Remotr.SourceGen.UseHandlerAttributes.KeyStrategy;
using Remotr.SourceGen.UseHandlerAttributes.Utils;
using Remotr.SourceGen.UseHandlerAttributes.Validators;
using System.Linq;
using System.Text;
using Remotr.SourceGen.Shared;
using Remotr.SourceGen.UseHandlerAttributes.HandlerGenerators;

namespace Remotr.SourceGen.UseHandlerAttributes.HandlerGenerators;

/// <summary>
/// Generates stateless handler code.
/// </summary>
public class HandlersGenerator
{
    private IStatelessHandlerGenerator? _handlerGenerator;
    private readonly HandlerTypeValidator _handlerTypeValidator;
    
    /// <summary>
    /// Initializes a new instance of the <see cref="HandlersGenerator"/> class.
    /// </summary>
    public HandlersGenerator()
    {
        _handlerTypeValidator = new HandlerTypeValidator();
    }

    /// <summary>
    /// Generates a stateless handler for the given interface and handler type.
    /// </summary>
    /// <param name="interfaceDeclaration">The interface declaration</param>
    /// <param name="handlerTypeSymbol">The handler type symbol</param>
    /// <param name="alias">The alias</param>
    /// <param name="fixedKey">The fixed key, if specified</param>
    /// <param name="findMethod">The find method name, if specified</param>
    /// <param name="usePrimaryKey">Whether to use the primary key</param>
    /// <param name="context">The source production context</param>
    public void GenerateHandlers(
        StringBuilder sourceBuilder,
        string namespaceName,
        string handlerName,
        string stateType,
        string interfaceName,
        List<ITypeSymbol> genericTypeArgs,
        List<ITypeSymbol> baseGenericTypeArgs,
        bool isCommandHandler,
        string alias,
        string? fixedKey,
        string? findMethod)
    {
        var genericTypeArgsString =  genericTypeArgs.Any() 
            ? $"<{string.Join(", ", genericTypeArgs.Select(t => t.ToString()))}>" 
            : "";

        // Initialize the appropriate handler generator
        _handlerGenerator = isCommandHandler ? new CommandHandlerGenerator() : new QueryHandlerGenerator();
        
        // Generate the handler based on the number of generic type arguments
        GenerateHandler(sourceBuilder, interfaceName, alias, handlerName, stateType, genericTypeArgsString,baseGenericTypeArgs, fixedKey, findMethod, namespaceName);
    }

    private void GenerateHandler(
        StringBuilder sourceBuilder, 
        string interfaceName, 
        string className, 
        string statefulHandlerName, 
        string stateType, 
        string genericTypeArgsString,
        List<ITypeSymbol> baseGenericTypeArgs,
        string? fixedKey,
        string? findMethod,
        string? namespaceName)
    {
        // Determine the key strategy
        IHandlerKeyStrategy keyStrategy;
        if (!string.IsNullOrEmpty(fixedKey))
        {
            keyStrategy = new FixedKeyStrategy(fixedKey!);
        }
        else if (!string.IsNullOrEmpty(findMethod))
        {
            bool hasInput = baseGenericTypeArgs.Count > 2; // For handlers with input
            string inputName = hasInput ? "input" : "";
            var fullInterfaceName = !string.IsNullOrEmpty(namespaceName) ? $"{namespaceName}.{interfaceName}" : interfaceName;
            keyStrategy = new StaticMethodKeyStrategy(fullInterfaceName, findMethod ?? "", hasInput, inputName);
        }
        else
        {
            // Default or explicit primary key
            keyStrategy = new PrimaryKeyStrategy();
        }

        switch (baseGenericTypeArgs.Count)
        {
            case 1: // ex EntityCommandHandler<TState>
                _handlerGenerator!.GenerateNoInputNoOutput(sourceBuilder, interfaceName, className, statefulHandlerName, stateType, genericTypeArgsString, keyStrategy);
                break;
            case 2: // ex EntityCommandHandler<TState, TOutput>
                _handlerGenerator!.GenerateNoInputWithOutput(sourceBuilder, interfaceName, className, statefulHandlerName, stateType, baseGenericTypeArgs[1].ToString(), genericTypeArgsString, keyStrategy);
                break;
            case 3: // ex EntityCommandHandler<TState, TInput, TOutput>
                _handlerGenerator!.GenerateWithInputAndOutput(sourceBuilder, interfaceName, className, statefulHandlerName, stateType, baseGenericTypeArgs[1].ToString(), baseGenericTypeArgs[2].ToString(), genericTypeArgsString, keyStrategy);
                break;
        }
    }
} 