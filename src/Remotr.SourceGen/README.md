# Remotr.SourceGen

This project contains source generators for the Remotr framework. The generators produce boilerplate code for implementing CQRS-based handlers and extensions.

## Project Structure

```
Remotr.SourceGen/
├── UseHandlerAttributes/               # Handler attributes source generator components
│   ├── HandlerAttributeIncrementalGenerator.cs # Main generator implementation
│   ├── HandlerAttributeExecutor.cs     # Orchestrates validation and generation
│   ├── HandlerGenerators/              # Specific handler code generators
│   ├── KeyStrategy/                    # Strategies for handling keys
│   ├── Utils/                          # Utility classes and helpers
│   └── Validators/                     # Validation components
├── UseShortcutsAttribute/                 # Remotr generator components
│   ├── UseShortcutserator.cs              # Main generator implementation
│   ├── RemotrExecutor.cs               # Orchestrates validation and generation
│   ├── RemotrAttributeGenerator.cs     # Generates attribute code
│   └── Utils/                          # Utility classes for the Remotr generator
├── StatelessExtensionGenerator/        # Generates extensions for stateless handlers
│   ├── IStatelessExtensionGenerator.cs # Interface for extension generators
│   ├── StatelessExtensionGeneratorComponent.cs # Main component
│   ├── RootCommandExtensionGenerator.cs # Command extensions
│   └── RootQueryExtensionGenerator.cs # Query extensions
├── StatefulExtensionGenerator/         # Generates extensions for stateful handlers
│   ├── IStatefulExtensionGenerator.cs  # Interface for extension generators
│   ├── StatefulExtensionGeneratorComponent.cs # Main component
│   ├── EntityCommandExtensionGenerator.cs # Command extensions
│   └── EntityQueryExtensionGenerator.cs # Query extensions
├── Shared/                             # Shared components and utilities
│   ├── AttributeGenerator.cs           # Base attribute generation
│   ├── BaseExtensionGenerator.cs       # Base class for extension generators
│   ├── ExtensionConfig.cs              # Configuration for extensions
│   ├── ExtensionsGeneratorExecutor.cs  # Executor for extension generators
│   └── IExtensionGenerator.cs          # Common interface for extension generators
└── Generators.cs                       # Main entry point for all generators
```

## Generators

### HandlerAttributeIncrementalGenerator (formerly CqrsCollectionGenerator)

This generator processes interfaces marked with handler attributes (such as `[UseCommandHandler]`, `[UseQueryHandler]`) and generates appropriate handler implementations.

The generator is organized into the following components:

1. **Entry Point**
   - `HandlerAttributeIncrementalGenerator`: The main entry point that implements `IIncrementalGenerator`.
   - `HandlerAttributeExecutor`: Orchestrates the validation and generation process.

2. **Validators**
   - Various validator classes to ensure correctness of inputs.

3. **Handler Generators**
   - Specific generators for different handler types.

### UseShortcutserator

The UseShortcutserator processes classes marked with the `[UseShortcuts]` attribute and generates extension methods for various handler types. It uses a plugin architecture where different handler generators can be registered.

### Extension Generators

The project includes generators for creating extension methods for both stateful and stateless handlers:

1. **StatelessExtensionGenerator**: Generates extension methods for stateless handlers.
2. **StatefulExtensionGenerator**: Generates extension methods for handlers that maintain state.

## Usage

To use these generators in your project, add a reference to the Remotr.SourceGen project and mark your interfaces or classes with the appropriate attributes.

### Handler Attribute Example

```csharp
// Interface with command and query handlers
[UseCommand(typeof(SetValueState), "SetValue")]
[UseCommand(typeof(MultiplyState), "Multiply")]
[UseCommand(typeof(DivideState), "Divide", fixedKey: "fixed-divide-key")]
[UseQuery(typeof(GetValueState<CalculatorState, string>), "GetValue")]
[UseQuery(typeof(GetPrimeFactorsState), "GetPrimeFactors", findMethod: nameof(GetPrimeFactorsKey))]
public interface ICalculatorAggregate : IAggregateRoot, IGrainWithStringKey
{
    // Optional key resolution method
    public static string GetPrimeFactorsKey(int input) 
    {
        return "Calculator";
    }
}
```

### Remotr Example

```csharp
// Stateful command handler implementation
[UseShortcuts]
public class MultiplyState : EntityCommandHandler<CalculatorState, double, double>
{
    public override async Task<double> Execute(double input)
    {
        await UpdateState(
            new() {
                Value = (await GetState()).Value * input 
            }
        );
        return input;
    }
}

// Stateful query handler implementation
[UseShortcuts]
public class GetPrimeFactorsState : EntityQueryHandler<CalculatorState, int, List<int>>
{
    public override async Task<List<int>> Execute(int input)
    {
        // Implementation
        return CalculatePrimeFactors(input);
    }
}
```

## Extending the Generators

To add new handler types:

1. For HandlerAttributeIncrementalGenerator, implement a new handler generator and register it.
2. For UseShortcutserator, extend the existing generator components or implement new ones.
3. For extension generators, implement the appropriate interface (IStatelessExtensionGenerator or IStatefulExtensionGenerator).

## Contributing

Contributions are welcome! Please follow the existing code style and add appropriate documentation comments. 