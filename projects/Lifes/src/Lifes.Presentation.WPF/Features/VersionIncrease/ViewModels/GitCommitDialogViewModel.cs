using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Lifes.Presentation.WPF.Features.VersionIncrease.ViewModels;

/// <summary>
/// ViewModel for the Git Commit Dialog.
/// </summary>
public partial class GitCommitDialogViewModel : ObservableObject
{
    [ObservableProperty]
    private string _commitMessage = string.Empty;

    [ObservableProperty]
    private ObservableCollection<string> _modifiedFiles = new();

    [ObservableProperty]
    private bool _pushToRemote = true;

    [ObservableProperty]
    private bool _isProcessing = false;

    [ObservableProperty]
    private string _statusMessage = string.Empty;

    /// <summary>
    /// Gets or sets the dialog result.
    /// </summary>
    public bool DialogResult { get; set; }

    /// <summary>
    /// Action to close the dialog.
    /// </summary>
    public Action? CloseAction { get; set; }

    /// <summary>
    /// Command to confirm and commit.
    /// </summary>
    [RelayCommand(CanExecute = nameof(CanCommit))]
    private void Commit()
    {
        DialogResult = true;
        CloseAction?.Invoke();
    }

    /// <summary>
    /// Command to cancel the dialog.
    /// </summary>
    [RelayCommand]
    private void Cancel()
    {
        DialogResult = false;
        CloseAction?.Invoke();
    }

    /// <summary>
    /// Determines whether the commit command can execute.
    /// </summary>
    private bool CanCommit()
    {
        return !string.IsNullOrWhiteSpace(CommitMessage) && !IsProcessing;
    }

    partial void OnCommitMessageChanged(string value)
    {
        CommitCommand.NotifyCanExecuteChanged();
    }

    partial void OnIsProcessingChanged(bool value)
    {
        CommitCommand.NotifyCanExecuteChanged();
    }
}
