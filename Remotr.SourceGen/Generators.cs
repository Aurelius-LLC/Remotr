using Microsoft.CodeAnalysis;
using Remotr.SourceGen.HandlerAttributes;
using Remotr.SourceGen.Remotr;

namespace Remotr.SourceGen;

/// <summary>
/// Main entry point for the Remotr.SourceGen generators.
/// This class provides access to all the source generators in the project.
/// </summary>
public static class Generators
{
    /// <summary>
    /// Gets the CqrsCollectionGenerator instance.
    /// </summary>
    /// <returns>A new instance of the CqrsCollectionIncrementalGenerator.</returns>
    public static IIncrementalGenerator GetCqrsCollectionGenerator() => new HandlerAttributeIncrementalGenerator();

    /// <summary>
    /// Gets the RemotrGenerator instance.
    /// </summary>
    /// <returns>A new instance of the RemotrGenerator.</returns>
    public static IIncrementalGenerator GetRemotrGenerator() => new RemotrGenerator();
} 