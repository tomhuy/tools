using ETLTools.Core.Models;

namespace ETLTools.Core.Interfaces;

/// <summary>
/// Manages navigation between ETL tool forms.
/// </summary>
public interface INavigationService
{
    /// <summary>Registers a tool so it appears in the navigation menu.</summary>
    void RegisterTool(ToolDefinition tool);

    /// <summary>Returns all registered tools in registration order.</summary>
    IEnumerable<ToolDefinition> GetAllTools();

    /// <summary>
    /// Navigates to the tool with the given <paramref name="toolId"/>.
    /// Does nothing when <paramref name="toolId"/> is already the current tool.
    /// </summary>
    void NavigateTo(string toolId);

    /// <summary>Gets the currently active tool, or <c>null</c> before any navigation.</summary>
    ToolDefinition? GetCurrentTool();

    /// <summary>Raised after the active tool changes.</summary>
    event EventHandler<ToolNavigatedEventArgs>? ToolNavigated;
}
