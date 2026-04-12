using System.Windows.Controls;
using Lifes.Presentation.WPF.Features.DashboardChart.Attributes;
using Lifes.Presentation.WPF.Features.DashboardChart.Interfaces;
using UserControl = System.Windows.Controls.UserControl;

namespace Lifes.Presentation.WPF.Features.DashboardChart.Views;

[DashboardBlock("AstrologyCell")]
public partial class AstrologyCellView : UserControl, IDashboardBlockView
{
    public AstrologyCellView()
    {
        InitializeComponent();
    }
}
