using System.Collections.ObjectModel;
using System.Text;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Lifes.Application.Common.Commands;
using Lifes.Application.Common.DTOs;
using Lifes.Application.Features.VersionIncrease.Commands;
using Lifes.Application.Features.VersionIncrease.DTOs;
using Lifes.Presentation.WPF.Features.VersionIncrease.Helpers;
using Lifes.Core.Interfaces;
using Lifes.Presentation.WPF.Constants;
using Lifes.Presentation.WPF.Features.VersionIncrease.Models;
using Lifes.Presentation.WPF.Features.VersionIncrease.ViewModels;
using Lifes.Presentation.WPF.Features.VersionIncrease.Views;
using Lifes.Presentation.WPF.Models;
using Microsoft.Extensions.Logging;
using WinForms = System.Windows.Forms;

namespace Lifes.Presentation.WPF.Features.VersionIncrease;

/// <summary>
/// View model for Version Increase Tool.
/// </summary>
public partial class VersionIncreaseViewModel : ObservableObject
{
    private readonly IScanProjectsCommand _scanProjectsCommand;
    private readonly IUpdateVersionsCommand _updateVersionsCommand;
    private readonly ILoadSettingsCommand _loadSettingsCommand;
    private readonly ISaveSettingsCommand _saveSettingsCommand;
    private readonly CommitChangesCommand _commitChangesCommand;
    private readonly IGitService _gitService;
    private readonly INavigationService _navigationService;
    private readonly ILogger<VersionIncreaseViewModel> _logger;
    private List<ProjectUpdateDto> _lastUpdateResults = new();

    [ObservableProperty]
    private string _baseDirectory = string.Empty;

    [ObservableProperty]
    private ObservableCollection<ProjectFileViewModel> _projectFiles = new();

    [ObservableProperty]
    private ObservableCollection<ProjectFileViewModel> _filteredProjectFiles = new();

    [ObservableProperty]
    private string _searchText = string.Empty;

    [ObservableProperty]
    private string _filterStatusText = string.Empty;

    [ObservableProperty]
    private int _foundCount;

    [ObservableProperty]
    private int _selectedCount;

    [ObservableProperty]
    private int _processedCount;

    [ObservableProperty]
    private int _totalCount;

    [ObservableProperty]
    private double _progressPercentage;

    [ObservableProperty]
    private bool _isProcessing;

    [ObservableProperty]
    private string _statusMessage = "Ready";

    [ObservableProperty]
    private bool _canCommit;

    [ObservableProperty]
    private ObservableCollection<ToolMenuItem> _navigationMenuItems = new();

    public VersionIncreaseViewModel(
        IScanProjectsCommand scanProjectsCommand,
        IUpdateVersionsCommand updateVersionsCommand,
        ILoadSettingsCommand loadSettingsCommand,
        ISaveSettingsCommand saveSettingsCommand,
        CommitChangesCommand commitChangesCommand,
        IGitService gitService,
        INavigationService navigationService,
        ILogger<VersionIncreaseViewModel> logger)
    {
        _scanProjectsCommand = scanProjectsCommand;
        _updateVersionsCommand = updateVersionsCommand;
        _loadSettingsCommand = loadSettingsCommand;
        _saveSettingsCommand = saveSettingsCommand;
        _commitChangesCommand = commitChangesCommand;
        _gitService = gitService;
        _navigationService = navigationService;
        _logger = logger;

        // Initialize with a default directory (current directory or user's Documents)
        BaseDirectory = Environment.CurrentDirectory;

        // Load last directory from settings
        _ = LoadLastDirectoryAsync();

        // Build navigation menu from registered tools
        InitializeNavigationMenu();

        // Subscribe to property changes
        PropertyChanged += (s, e) =>
        {
            if (e.PropertyName == nameof(SearchText))
            {
                ApplyFilter();
            }
            else if (e.PropertyName == nameof(BaseDirectory))
            {
                _ = SaveLastDirectoryAsync();
            }
        };
    }

    /// <summary>
    /// Builds <see cref="NavigationMenuItems"/> from the tools registered in the navigation service.
    /// </summary>
    private void InitializeNavigationMenu()
    {
        NavigationMenuItems = new ObservableCollection<ToolMenuItem>(
            _navigationService.GetAllTools().Select(t => new ToolMenuItem
            {
                Id = t.Id,
                Name = t.Name,
                Description = t.Description,
                IsActive = t.Id == ToolIds.VersionIncrease,
                NavigateCommand = new RelayCommand(
                    () => _navigationService.NavigateTo(t.Id),
                    () => t.Id != ToolIds.VersionIncrease)
            })
        );
    }

    /// <summary>
    /// Loads last directory from settings.
    /// </summary>
    private async Task LoadLastDirectoryAsync()
    {
        try
        {
            var result = await _loadSettingsCommand.ExecuteAsync();
            if (result.IsSuccess && result.Value != null)
            {
                var settings = result.Value;
                if (!string.IsNullOrWhiteSpace(settings.LastDirectory))
                {
                    BaseDirectory = settings.LastDirectory;
                    _logger.LogInformation("Loaded last directory: {Directory}", settings.LastDirectory);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to load last directory from settings");
        }
    }

    /// <summary>
    /// Saves last directory to settings.
    /// </summary>
    private async Task SaveLastDirectoryAsync()
    {
        try
        {
            if (string.IsNullOrWhiteSpace(BaseDirectory))
            {
                return;
            }

            var settings = new AppSettingsDto
            {
                LastDirectory = BaseDirectory,
                Version = "1.0"
            };

            var result = await _saveSettingsCommand.ExecuteAsync(settings);
            if (result.IsSuccess)
            {
                _logger.LogDebug("Saved last directory: {Directory}", BaseDirectory);
            }
            else
            {
                _logger.LogWarning("Failed to save last directory: {Error}", result.Error);
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to save last directory to settings");
        }
    }

    /// <summary>
    /// Applies filter to project files based on search text.
    /// </summary>
    private void ApplyFilter()
    {
        var filtered = ProjectFilterHelper.Filter(ProjectFiles, SearchText);
        FilteredProjectFiles = new ObservableCollection<ProjectFileViewModel>(filtered);
        
        var isFiltered = !string.IsNullOrWhiteSpace(SearchText);
        FilterStatusText = ProjectFilterHelper.GetFilterStatusText(
            FilteredProjectFiles.Count,
            ProjectFiles.Count,
            isFiltered);
        
        _logger.LogDebug("Applied filter. Showing {Filtered} of {Total} projects", 
            FilteredProjectFiles.Count, ProjectFiles.Count);
    }

    /// <summary>
    /// Gets whether search text exists.
    /// </summary>
    public bool HasSearchText => !string.IsNullOrWhiteSpace(SearchText);

    /// <summary>
    /// Command to browse for a directory.
    /// </summary>
    [RelayCommand]
    private void BrowseDirectory()
    {
        var dialog = new WinForms.FolderBrowserDialog
        {
            Description = "Select Base Directory",
            SelectedPath = BaseDirectory,
            UseDescriptionForTitle = true
        };

        if (dialog.ShowDialog() == WinForms.DialogResult.OK)
        {
            BaseDirectory = dialog.SelectedPath;
            _logger.LogInformation("Base directory changed to: {BaseDirectory}", BaseDirectory);
            // Settings will be saved automatically via PropertyChanged event
        }
    }

    /// <summary>
    /// Command to clear search text.
    /// </summary>
    [RelayCommand]
    private void ClearSearch()
    {
        SearchText = string.Empty;
        _logger.LogDebug("Search cleared");
    }

    /// <summary>
    /// Command to scan for projects.
    /// </summary>
    [RelayCommand(CanExecute = nameof(CanScanProjects))]
    private async Task ScanProjectsAsync()
    {
        try
        {
            IsProcessing = true;
            StatusMessage = "Scanning projects...";
            ProjectFiles.Clear();
            FoundCount = 0;
            SelectedCount = 0;

            _logger.LogInformation("Scanning projects in: {BaseDirectory}", BaseDirectory);

            var result = await _scanProjectsCommand.ExecuteAsync(BaseDirectory);

            if (!result.IsSuccess)
            {
                StatusMessage = $"Error: {result.Error}";
                System.Windows.MessageBox.Show(result.Error, "Scan Error", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                return;
            }

            var projects = result.Value ?? Enumerable.Empty<ProjectFileDto>();
            foreach (var project in projects)
            {
                ProjectFiles.Add(new ProjectFileViewModel
                {
                    FileName = project.FileName,
                    CurrentVersion = project.CurrentVersion,
                    FullPath = project.FullPath,
                    RelativePath = project.RelativePath,
                    Status = "Ready",
                    IsSelected = false
                });
            }

            FoundCount = ProjectFiles.Count;
            StatusMessage = $"Found {FoundCount} projects";
            
            // Apply filter to populate FilteredProjectFiles
            ApplyFilter();
            
            _logger.LogInformation("Found {Count} projects", FoundCount);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to scan projects");
            StatusMessage = "Scan failed";
            System.Windows.MessageBox.Show($"Failed to scan projects: {ex.Message}", "Error", 
                System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
        }
        finally
        {
            IsProcessing = false;
        }
    }

    private bool CanScanProjects() => !IsProcessing && !string.IsNullOrWhiteSpace(BaseDirectory);

    /// <summary>
    /// Command to select all projects (only visible/filtered projects).
    /// </summary>
    [RelayCommand]
    private void SelectAll()
    {
        // Only select visible (filtered) projects
        foreach (var project in FilteredProjectFiles)
        {
            project.IsSelected = true;
        }
        UpdateSelectedCount();
        _logger.LogDebug("Selected {Count} filtered projects", FilteredProjectFiles.Count);
    }

    /// <summary>
    /// Command to deselect all projects (only visible/filtered projects).
    /// </summary>
    [RelayCommand]
    private void DeselectAll()
    {
        // Only deselect visible (filtered) projects
        foreach (var project in FilteredProjectFiles)
        {
            project.IsSelected = false;
        }
        UpdateSelectedCount();
        _logger.LogDebug("Deselected {Count} filtered projects", FilteredProjectFiles.Count);
    }

    /// <summary>
    /// Updates the selected count.
    /// </summary>
    public void UpdateSelectedCount()
    {
        SelectedCount = ProjectFiles.Count(p => p.IsSelected);
        
        // Trigger CanExecute re-evaluation for IncreaseVersion command
        IncreaseVersionCommand.NotifyCanExecuteChanged();
    }

    /// <summary>
    /// Command to increase versions.
    /// </summary>
    [RelayCommand(CanExecute = nameof(CanIncreaseVersion))]
    private async Task IncreaseVersionAsync()
    {
        try
        {
            var selectedProjects = ProjectFiles.Where(p => p.IsSelected).ToList();
            
            // Confirmation dialog
            var result = System.Windows.MessageBox.Show(
                $"Update version for {selectedProjects.Count} projects to {DateTime.Now:yyyy.M.d}.{{auto}}?",
                "Confirm Version Update",
                System.Windows.MessageBoxButton.YesNo,
                System.Windows.MessageBoxImage.Question);

            if (result != System.Windows.MessageBoxResult.Yes)
            {
                return;
            }

            IsProcessing = true;
            StatusMessage = "Updating versions...";
            ProcessedCount = 0;
            TotalCount = selectedProjects.Count;
            ProgressPercentage = 0;

            _logger.LogInformation("Starting version update for {Count} projects", selectedProjects.Count);

            // Convert to DTOs
            var projectDtos = selectedProjects.Select(p => new ProjectFileDto
            {
                FileName = p.FileName,
                FullPath = p.FullPath,
                RelativePath = p.RelativePath,
                CurrentVersion = p.CurrentVersion
            }).ToList();

            // Progress reporter
            var progress = new Progress<(int current, int total)>(update =>
            {
                ProcessedCount = update.current;
                TotalCount = update.total;
                ProgressPercentage = (double)update.current / update.total * 100;
            });

            // Execute update command
            var updateResult = await _updateVersionsCommand.ExecuteAsync(projectDtos, DateTime.Now, progress);

            if (!updateResult.IsSuccess)
            {
                StatusMessage = $"Update failed: {updateResult.Error}";
                System.Windows.MessageBox.Show(updateResult.Error, "Update Error", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                return;
            }

            var summary = updateResult.Value!;
            
            // Update UI with new versions
            foreach (var update in summary.Updates)
            {
                var projectVm = ProjectFiles.FirstOrDefault(p => p.FileName == update.ProjectName);
                if (projectVm != null)
                {
                    if (update.Success)
                    {
                        projectVm.CurrentVersion = update.NewVersion;
                        projectVm.Status = "Updated";
                        _logger.LogInformation("Updated {ProjectName}: {OldVersion} → {NewVersion}",
                            update.ProjectName, update.OldVersion, update.NewVersion);
                    }
                    else
                    {
                        projectVm.Status = "Failed";
                        _logger.LogWarning("Failed to update {ProjectName}: {Error}",
                            update.ProjectName, update.ErrorMessage);
                    }
                }
            }

            StatusMessage = summary.Summary;

            // Store update results for commit message generation
            _lastUpdateResults = summary.Updates.Where(u => u.Success).ToList();

            // Enable commit button if updates succeeded
            CanCommit = _lastUpdateResults.Any();

            System.Windows.MessageBox.Show(
                $"{summary.Summary}\n\nSucceeded: {summary.SuccessCount}\nFailed: {summary.FailedCount}",
                "Update Complete",
                System.Windows.MessageBoxButton.OK,
                summary.IsSuccess ? System.Windows.MessageBoxImage.Information : System.Windows.MessageBoxImage.Warning);

            _logger.LogInformation(summary.Summary);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update versions");
            StatusMessage = "Update failed";
            System.Windows.MessageBox.Show($"Failed to update versions: {ex.Message}", "Error",
                System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
        }
        finally
        {
            IsProcessing = false;
            ProgressPercentage = 0;
        }
    }

    private bool CanIncreaseVersion() => !IsProcessing && ProjectFiles.Any(p => p.IsSelected);

    /// <summary>
    /// Command to commit and push changes.
    /// </summary>
    [RelayCommand(CanExecute = nameof(CanCommitAndPush))]
    private async Task CommitAndPushAsync()
    {
        try
        {
            // Check if Git repository exists
                var isRepoResult = await _gitService.IsGitRepositoryAsync(BaseDirectory);
            if (!isRepoResult.IsSuccess || !isRepoResult.Value)
            {
                System.Windows.MessageBox.Show(
                    "Current directory is not a Git repository",
                    "Git Repository Not Found",
                    System.Windows.MessageBoxButton.OK,
                    System.Windows.MessageBoxImage.Warning);
                return;
            }

            // Get modified files
            var modifiedFilesResult = await _gitService.GetModifiedFilesAsync(BaseDirectory);
            if (!modifiedFilesResult.IsSuccess)
            {
                System.Windows.MessageBox.Show(
                    $"Failed to get modified files: {modifiedFilesResult.Error}",
                    "Error",
                    System.Windows.MessageBoxButton.OK,
                    System.Windows.MessageBoxImage.Error);
                return;
            }

            var modifiedFiles = modifiedFilesResult.Value.ToList();
            if (!modifiedFiles.Any())
            {
                System.Windows.MessageBox.Show(
                    "No changes to commit",
                    "No Changes",
                    System.Windows.MessageBoxButton.OK,
                    System.Windows.MessageBoxImage.Information);
                return;
            }

            // Generate commit message
            var commitMessage = GenerateCommitMessage(_lastUpdateResults);

            // Create and show dialog
            var dialogViewModel = new GitCommitDialogViewModel
            {
                CommitMessage = commitMessage,
                ModifiedFiles = new ObservableCollection<string>(modifiedFiles),
                PushToRemote = false
            };

            var dialog = new GitCommitDialog(dialogViewModel)
            {
                Owner = System.Windows.Application.Current.MainWindow
            };
            
            dialog.ShowDialog();
            
            if (dialogViewModel.DialogResult != true)
            {
                _logger.LogInformation("Commit cancelled by user");
                return;
            }

            // Execute commit and push
            IsProcessing = true;
            StatusMessage = "Committing changes...";

            var progress = new Progress<string>(message =>
            {
                StatusMessage = message;
            });

            var commitDto = new CommitChangesDto
            {
                RepositoryPath = BaseDirectory,
                ModifiedFiles = modifiedFiles,
                CommitMessage = dialogViewModel.CommitMessage,
                PushToRemote = dialogViewModel.PushToRemote,
                RemoteName = "origin"
            };

            var result = await _commitChangesCommand.ExecuteAsync(commitDto, progress);

            if (!result.IsSuccess)
            {
                StatusMessage = $"Commit failed: {result.Error}";
                System.Windows.MessageBox.Show(
                    result.Error,
                    "Commit Failed",
                    System.Windows.MessageBoxButton.OK,
                    System.Windows.MessageBoxImage.Error);
                return;
            }

            var commitResult = result.Value!;
            StatusMessage = commitResult.Summary;

            // Reset commit state after successful commit
            CanCommit = false;
            _lastUpdateResults.Clear();

            System.Windows.MessageBox.Show(
                commitResult.Summary,
                "Commit Complete",
                System.Windows.MessageBoxButton.OK,
                commitResult.CommitSuccess && commitResult.PushSuccess
                    ? System.Windows.MessageBoxImage.Information
                    : System.Windows.MessageBoxImage.Warning);

            _logger.LogInformation("Commit completed: {Summary}", commitResult.Summary);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to commit and push");
            StatusMessage = "Commit failed";
            System.Windows.MessageBox.Show(
                $"Failed to commit and push: {ex.Message}",
                "Error",
                System.Windows.MessageBoxButton.OK,
                System.Windows.MessageBoxImage.Error);
        }
        finally
        {
            IsProcessing = false;
        }
    }

    private bool CanCommitAndPush() => !IsProcessing && CanCommit;

    partial void OnCanCommitChanged(bool value)
    {
        CommitAndPushCommand.NotifyCanExecuteChanged();
    }

    partial void OnIsProcessingChanged(bool value)
    {
        CommitAndPushCommand.NotifyCanExecuteChanged();
    }

    /// <summary>
    /// Generates a commit message from project updates.
    /// </summary>
    /// <param name="updates">The list of project updates.</param>
    /// <returns>A formatted commit message.</returns>
    private static string GenerateCommitMessage(IEnumerable<ProjectUpdateDto> updates)
    {
        var updatesList = updates.ToList();
        if (!updatesList.Any())
        {
            return "[AI:0] increase version";
        }

        var count = updatesList.Count;
        var firstUpdate = updatesList.First();
        var newVersion = firstUpdate.NewVersion;

        var sb = new StringBuilder();
        sb.AppendLine($"[AI:0] increase version to {newVersion} for {count} project{(count > 1 ? "s" : "")}");
        // sb.AppendLine();
        //
        // foreach (var update in updatesList)
        // {
        //     sb.AppendLine($"- {update.ProjectName}: {update.OldVersion} → {update.NewVersion}");
        // }

        return sb.ToString().TrimEnd();
    }
}
