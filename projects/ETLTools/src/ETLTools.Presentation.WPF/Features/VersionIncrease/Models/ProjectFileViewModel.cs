using CommunityToolkit.Mvvm.ComponentModel;

namespace ETLTools.Presentation.WPF.Features.VersionIncrease.Models;

/// <summary>
/// View model for individual project file.
/// </summary>
public partial class ProjectFileViewModel : ObservableObject
{
    [ObservableProperty]
    private bool _isSelected;

    [ObservableProperty]
    private string _fileName = string.Empty;

    [ObservableProperty]
    private string _currentVersion = string.Empty;

    [ObservableProperty]
    private string _status = "Ready";

    [ObservableProperty]
    private string _relativePath = string.Empty;

    public string FullPath { get; set; } = string.Empty;
}
