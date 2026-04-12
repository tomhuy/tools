using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Lifes.Presentation.WPF.Models;

/// <summary>
/// Represents a single item shown inside the navigation dropdown menu.
/// </summary>
public partial class ToolMenuItem : ObservableObject
{
    [ObservableProperty]
    private string _id = string.Empty;

    [ObservableProperty]
    private string _name = string.Empty;

    [ObservableProperty]
    private string _description = string.Empty;

    [ObservableProperty]
    private string? _iconPath;

    /// <summary>Gets or sets whether this item represents the currently active tool.</summary>
    [ObservableProperty]
    private bool _isActive;

    /// <summary>Gets or sets the command executed when the user clicks this item.</summary>
    public ICommand? NavigateCommand { get; set; }
}
