namespace Lifes.Core.Models;

/// <summary>
/// Provides data for the <see cref="Lifes.Core.Interfaces.INavigationService.ToolNavigated"/> event.
/// </summary>
public class ToolNavigatedEventArgs : EventArgs
{
    /// <summary>Gets the tool that was navigated to.</summary>
    public ToolDefinition Tool { get; }

    public ToolNavigatedEventArgs(ToolDefinition tool)
    {
        Tool = tool;
    }
}
