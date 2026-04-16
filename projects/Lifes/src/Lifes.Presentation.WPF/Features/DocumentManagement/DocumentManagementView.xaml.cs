using System.Windows.Controls;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using System.Windows.Controls.Primitives;

namespace Lifes.Presentation.WPF.Features.DocumentManagement;

public partial class DocumentManagementView : System.Windows.Controls.UserControl
{
    private bool _userResized = false;

    public DocumentManagementView()
    {
        InitializeComponent();
        Loaded += DocumentManagementView_Loaded;
    }

    private void DocumentManagementView_Loaded(object sender, RoutedEventArgs e)
    {
        if (DataContext is DocumentManagementViewModel viewModel)
        {
             viewModel.LoadDataCommand.Execute(null);
        }
    }

    private void MainScroll_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        if (!_userResized && e.WidthChanged)
        {
            var vm = DataContext as DocumentManagementViewModel;
            if (vm != null && vm.DayHeaders.Count > 0)
            {
                double daysWidth = vm.DayHeaders.Count * 25; // 25 is exact day cell width
                double desiredWidth = e.NewSize.Width - daysWidth - 5; // Account for borders
                if (desiredWidth < 200) desiredWidth = 200; // Hard minimum
                TitleColDef.Width = new GridLength(desiredWidth);
            }
        }
    }

    private void GridSplitter_DragStarted(object sender, DragStartedEventArgs e)
    {
        _userResized = true;
    }
}
