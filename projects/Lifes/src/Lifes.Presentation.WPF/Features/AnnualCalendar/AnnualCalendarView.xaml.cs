using System.Windows;
using System.Windows.Controls;
using Microsoft.Extensions.DependencyInjection;

namespace Lifes.Presentation.WPF.Features.AnnualCalendar;

public partial class AnnualCalendarView : System.Windows.Controls.UserControl
{
    public AnnualCalendarView()
    {
        InitializeComponent();
        Loaded += AnnualCalendarView_Loaded;
    }

    private void AnnualCalendarView_Loaded(object sender, RoutedEventArgs e)
    {
        if (DataContext is AnnualCalendarViewModel viewModel)
        {
             viewModel.LoadDataCommand.Execute(null);
        }
    }
}
