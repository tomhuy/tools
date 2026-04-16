using System.Windows;
using System.Windows.Controls;

namespace Lifes.Presentation.WPF.Features.AnnualCalendar;

public partial class MonthlyCalendarView : System.Windows.Controls.UserControl
{
    public MonthlyCalendarView()
    {
        InitializeComponent();
        Loaded += MonthlyCalendarView_Loaded;
    }

    private void MonthlyCalendarView_Loaded(object sender, RoutedEventArgs e)
    {
        if (DataContext is MonthlyCalendarViewModel viewModel)
        {
            viewModel.LoadDataCommand.Execute(null);
        }
    }
}
