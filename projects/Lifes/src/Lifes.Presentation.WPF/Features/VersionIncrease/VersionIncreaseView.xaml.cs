namespace Lifes.Presentation.WPF.Features.VersionIncrease;

/// <summary>
/// Interaction logic for VersionIncreaseView.xaml
/// </summary>
public partial class VersionIncreaseView : System.Windows.Controls.UserControl
{
    public VersionIncreaseView()
    {
        InitializeComponent();
    }

    /// <summary>
    /// Event handler for checkbox changed to update selected count.
    /// </summary>
    private void CheckBox_Changed(object sender, System.Windows.RoutedEventArgs e)
    {
        if (DataContext is VersionIncreaseViewModel viewModel)
        {
            viewModel.UpdateSelectedCount();
        }
    }
}
