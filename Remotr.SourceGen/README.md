# Remotr.SourceGen

This project contains source generators for the Remotr framework. The generators are organized into the following components:

## Project Structure

```
Remotr.SourceGen/
├── CqrsCollection/                  # CQRS Collection Generator components
│   ├── CqrsCollectionBase.cs        # Base class with common utilities
│   ├── CqrsCodeTemplates.cs         # Templates for code generation
│   ├── CqrsCollectionGenerator.cs   # Main generator implementation
│   └── StatelessHandlerGenerator.cs # Generator for stateless handlers
├── Remotr/                          # Remotr Generator components
│   ├── BaseHandlerGenerator.cs      # Base class for handler generators
│   ├── IHandlerGenerator.cs         # Interface for handler generators
│   ├── RemotrGenerator.cs           # Main generator implementation
│   └── StatelessCommandHandlerGenerator.cs # Example handler generator
└── Generators.cs                    # Main entry point for all generators
```

## Generators

### CqrsCollectionGenerator

The CqrsCollectionGenerator processes interfaces marked with the `[CqrsCollection]` attribute and generates appropriate handler implementations. It has been refactored to follow SOLID principles:

- **Single Responsibility Principle**: Each class has a single responsibility
- **Open/Closed Principle**: The generator is open for extension but closed for modification
- **Liskov Substitution Principle**: Subtypes can be used in place of their base types
- **Interface Segregation Principle**: Clients only depend on the interfaces they use
- **Dependency Inversion Principle**: High-level modules depend on abstractions

### RemotrGenerator

The RemotrGenerator processes classes marked with the `[RemotrGen]` attribute and generates extension methods for various handler types. It uses a plugin architecture where different handler generators can be registered.

## Usage

To use these generators in your project, add a reference to the Remotr.SourceGen project and mark your interfaces or classes with the appropriate attributes.

### CQRS Collection Example

```csharp
[CqrsCollection(typeof(ICommandHandler<MyState>), "MyCommand")]
public interface IMyCommand
{
    Task Execute(CancellationToken cancellationToken = default);
}
```

### Remotr Example

```csharp
[RemotrGen]
public class MyHandler : ICommandHandler<MyState>
{
    public Task Execute(MyState state, CancellationToken cancellationToken = default)
    {
        // Implementation
    }
}
```

## Extending the Generators

To add new handler types:

1. For CqrsCollectionGenerator, extend the StatelessHandlerGenerator or create a new generator
2. For RemotrGenerator, implement the IHandlerGenerator interface and register it in the RemotrGenerator constructor

## Contributing

Contributions are welcome! Please follow the existing code style and add appropriate documentation comments.

## CqrsCollection Source Generator

The CqrsCollection source generator is responsible for generating stateless handler classes for CQRS operations. It has been refactored to follow SOLID principles with a clear separation of concerns.

### Architecture

The generator is organized into the following components:

1. **Entry Point**
   - `CqrsCollectionIncrementalGenerator`: The main entry point that implements `IIncrementalGenerator`.
   - `CqrsCollectionExecutor`: Orchestrates the validation and generation process.

2. **Validators**
   - `InterfaceValidator`: Validates that interfaces implement required base interfaces.
   - `AttributeValidator`: Validates attribute arguments.
   - `HandlerTypeValidator`: Validates handler types.

3. **Generators**
   - `AttributeGenerator`: Generates the CqrsCollectionAttribute.
   - `StatelessHandlerGenerator`: Coordinates the generation of stateless handlers.
   - `CommandHandlerGenerator`: Generates command handler code.
   - `QueryHandlerGenerator`: Generates query handler code.

4. **Utilities**
   - `SyntaxTargetIdentifier`: Identifies syntax targets for generation.
   - `SemanticTargetIdentifier`: Identifies semantic targets for generation.
   - `TypeUtils`: Utility methods for working with types.

### Flow

1. The `CqrsCollectionIncrementalGenerator` initializes the generator and registers the attribute source.
2. It identifies interfaces with the CqrsCollection attribute.
3. For each interface, it calls the `CqrsCollectionExecutor` to validate and generate code.
4. The executor validates the interface, attribute arguments, and handler type.
5. If all validations pass, it generates the stateless handler code. 