using System.Windows.Controls;
using Lifes.Presentation.WPF.Features.DashboardChart.Interfaces;
using UserControl = System.Windows.Controls.UserControl;


namespace Lifes.Presentation.WPF.Features.DashboardChart.Views;

public partial class DefaultDashboardBlockView : UserControl, IDashboardBlockView
{
    public DefaultDashboardBlockView()
    {
        InitializeComponent();
    }
}
