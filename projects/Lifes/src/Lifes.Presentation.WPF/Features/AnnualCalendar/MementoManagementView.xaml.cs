using System.Windows;
using System.Windows.Controls;
using Lifes.Core.Models;

namespace Lifes.Presentation.WPF.Features.AnnualCalendar;

public partial class MementoManagementView : System.Windows.Controls.UserControl
{
    public MementoManagementView()
    {
        InitializeComponent();
        Loaded += MementoManagementView_Loaded;
    }

    private void MementoManagementView_Loaded(object sender, RoutedEventArgs e)
    {
        if (DataContext is MementoManagementViewModel viewModel)
        {
            viewModel.LoadDataCommand.Execute(null);
        }
    }
}
