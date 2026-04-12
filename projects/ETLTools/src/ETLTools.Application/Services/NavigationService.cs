using ETLTools.Core.Interfaces;
using ETLTools.Core.Models;

namespace ETLTools.Application.Services;

/// <summary>
/// Manages registration and navigation between ETL tool forms.
/// Thread-safe: all state mutations are guarded by a lock.
/// </summary>
public class NavigationService : INavigationService
{
    private readonly object _lock = new();
    private readonly Dictionary<string, ToolDefinition> _tools = new();
    private ToolDefinition? _currentTool;

    public event EventHandler<ToolNavigatedEventArgs>? ToolNavigated;

    /// <inheritdoc/>
    public void RegisterTool(ToolDefinition tool)
    {
        ArgumentNullException.ThrowIfNull(tool);
        lock (_lock)
        {
            _tools[tool.Id] = tool;
        }
    }

    /// <inheritdoc/>
    public IEnumerable<ToolDefinition> GetAllTools()
    {
        lock (_lock)
        {
            return _tools.Values.ToList();
        }
    }

    /// <inheritdoc/>
    public void NavigateTo(string toolId)
    {
        ToolDefinition? tool;
        lock (_lock)
        {
            if (!_tools.TryGetValue(toolId, out tool)) return;
            if (_currentTool?.Id == toolId) return;
            _currentTool = tool;
        }

        ToolNavigated?.Invoke(this, new ToolNavigatedEventArgs(tool));
    }

    /// <inheritdoc/>
    public ToolDefinition? GetCurrentTool()
    {
        lock (_lock)
        {
            return _currentTool;
        }
    }
}
