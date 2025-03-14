
namespace Remotr.SourceGen.Shared;

/// <summary>
/// Configuration for generating handler extensions
/// </summary>
public class ExtensionConfig
{
    public string BaseBuilderType { get; set; } = string.Empty;
    public string BuilderType { get; set; } = string.Empty;
    public string HandlerType { get; set; } = string.Empty;
    public string OtherHandlerType { get; set; } = string.Empty;
    public bool IncludeOtherHandler { get; set; } = true;
}