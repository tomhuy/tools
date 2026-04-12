using System.Windows;
using System.Windows.Controls;
using Lifes.Domain.Features.DashboardChart.Entities;
using Lifes.Presentation.WPF.Features.DashboardChart.Registries;
using Lifes.Presentation.WPF.Features.DashboardChart.Views;

namespace Lifes.Presentation.WPF.Features.DashboardChart.Controls;

public class DashboardBlockHost : ContentControl
{
    public static readonly DependencyProperty BlockProperty = DependencyProperty.Register(
        nameof(Block), typeof(DashboardBlock), typeof(DashboardBlockHost), new PropertyMetadata(null, OnBlockChanged));

    public DashboardBlock? Block
    {
        get => (DashboardBlock)GetValue(BlockProperty);
        set => SetValue(BlockProperty, value);
    }

    private static void OnBlockChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is DashboardBlockHost host)
        {
            host.UpdateView();
        }
    }

    private void UpdateView()
    {
        if (Block == null)
        {
            Content = null;
            return;
        }

        // Try to create the custom view mapped to this BlockType
        var view = DashboardViewRegistry.CreateView(Block.BlockType);

        // Fallback to DefaultDashboardBlockView if no custom view is mapped
        if (view == null)
        {
            view = new DefaultDashboardBlockView();
        }

        view.DataContext = Block;
        Content = view;
    }
}
