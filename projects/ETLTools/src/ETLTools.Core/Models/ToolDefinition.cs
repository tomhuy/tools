namespace ETLTools.Core.Models;

/// <summary>
/// Defines a tool's metadata for navigation.
/// </summary>
public class ToolDefinition
{
    /// <summary>Gets or sets the unique tool identifier.</summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>Gets or sets the display name.</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>Gets or sets the short description.</summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>Gets or sets the icon path (optional).</summary>
    public string? IconPath { get; set; }
}
