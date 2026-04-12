using ETLTools.Presentation.WPF.Features.VersionIncrease.ViewModels;
using System.Windows;

namespace ETLTools.Presentation.WPF.Features.VersionIncrease.Views;

/// <summary>
/// Interaction logic for GitCommitDialog.xaml
/// </summary>
public partial class GitCommitDialog : Window
{
    public GitCommitDialog(GitCommitDialogViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
        viewModel.CloseAction = Close;
    }

    /// <summary>
    /// Gets the ViewModel.
    /// </summary>
    public GitCommitDialogViewModel ViewModel => (GitCommitDialogViewModel)DataContext;
}
