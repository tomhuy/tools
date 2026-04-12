using System.Windows.Controls;
using System.Windows;
using UserControl = System.Windows.Controls.UserControl;

namespace Lifes.Presentation.WPF.Features.DashboardChart;

public partial class DashboardChartView : UserControl
{
    public DashboardChartView()
    {
        InitializeComponent();
        Loaded += OnLoaded;
    }
    
    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        if (DataContext is DashboardChartViewModel viewModel)
        {
            _ = viewModel.LoadDataCommand.ExecuteAsync(null);
        }
    }
}
