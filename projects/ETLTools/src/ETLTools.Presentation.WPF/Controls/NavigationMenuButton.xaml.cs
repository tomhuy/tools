using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using ETLTools.Presentation.WPF.Models;
using Key = System.Windows.Input.Key;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;
using MouseEventArgs = System.Windows.Input.MouseEventArgs;
using UserControl = System.Windows.Controls.UserControl;

namespace ETLTools.Presentation.WPF.Controls;

/// <summary>
/// A button that reveals a navigation dropdown on hover so the user can
/// quickly switch between registered ETL tool forms.
/// </summary>
public partial class NavigationMenuButton : UserControl
{
    // -------------------------------------------------------------------
    //  Dependency Properties
    // -------------------------------------------------------------------

    public static readonly DependencyProperty ToolItemsProperty =
        DependencyProperty.Register(
            nameof(ToolItems),
            typeof(ObservableCollection<ToolMenuItem>),
            typeof(NavigationMenuButton),
            new PropertyMetadata(null, OnToolItemsChanged));

    /// <summary>The list of tool menu items to display in the dropdown.</summary>
    public ObservableCollection<ToolMenuItem>? ToolItems
    {
        get => (ObservableCollection<ToolMenuItem>?)GetValue(ToolItemsProperty);
        set => SetValue(ToolItemsProperty, value);
    }

    private static void OnToolItemsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is NavigationMenuButton btn)
        {
            btn.PART_ItemsControl.ItemsSource = e.NewValue as ObservableCollection<ToolMenuItem>;
        }
    }

    // -------------------------------------------------------------------
    //  Fields
    // -------------------------------------------------------------------

    private readonly DispatcherTimer _closeTimer;

    // -------------------------------------------------------------------
    //  Constructor
    // -------------------------------------------------------------------

    public NavigationMenuButton()
    {
        InitializeComponent();

        _closeTimer = new DispatcherTimer
        {
            Interval = TimeSpan.FromMilliseconds(300)
        };
        _closeTimer.Tick += OnCloseTimerTick;

        // Bind popup border DataContext to 'this' so that
        // {Binding ToolItems} inside the Popup resolves against this UserControl.
        Loaded += (_, _) => PART_PopupBorder.DataContext = this;
    }

    // -------------------------------------------------------------------
    //  Timer
    // -------------------------------------------------------------------

    private void OnCloseTimerTick(object? sender, EventArgs e)
    {
        _closeTimer.Stop();
        AnimateClose();
    }

    // -------------------------------------------------------------------
    //  Mouse event handlers
    // -------------------------------------------------------------------

    private void OnNavButtonMouseEnter(object sender, MouseEventArgs e)
    {
        _closeTimer.Stop();
        OpenMenu();
    }

    private void OnNavButtonMouseLeave(object sender, MouseEventArgs e)
    {
        _closeTimer.Start();
    }

    private void OnPopupMouseEnter(object sender, MouseEventArgs e)
    {
        _closeTimer.Stop();
    }

    private void OnPopupMouseLeave(object sender, MouseEventArgs e)
    {
        _closeTimer.Start();
    }

    // -------------------------------------------------------------------
    //  Keyboard handler
    // -------------------------------------------------------------------

    private void OnPopupKeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Escape)
        {
            _closeTimer.Stop();
            AnimateClose();
            e.Handled = true;
        }
    }

    // -------------------------------------------------------------------
    //  Menu item click — close after any item is clicked
    // -------------------------------------------------------------------

    private void OnMenuItemClicked(object sender, RoutedEventArgs e)
    {
        _closeTimer.Stop();
        AnimateClose();
    }

    // -------------------------------------------------------------------
    //  Open / Close helpers
    // -------------------------------------------------------------------

    private void OpenMenu()
    {
        if (PART_NavPopup.IsOpen)
        {
            // Already open: stop any in-progress close animation
            PART_PopupBorder.BeginAnimation(OpacityProperty, null);
            PART_PopupBorder.Opacity = 1;
            return;
        }

        PART_PopupBorder.Opacity = 0;
        PART_NavPopup.IsOpen = true;

        var anim = new DoubleAnimation(0, 1, new Duration(TimeSpan.FromMilliseconds(180)))
        {
            EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut }
        };
        PART_PopupBorder.BeginAnimation(OpacityProperty, anim);
    }

    private void AnimateClose()
    {
        if (!PART_NavPopup.IsOpen) return;

        var anim = new DoubleAnimation(PART_PopupBorder.Opacity, 0,
            new Duration(TimeSpan.FromMilliseconds(160)))
        {
            EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseIn }
        };

        anim.Completed += (_, _) =>
        {
            PART_NavPopup.IsOpen = false;
            PART_PopupBorder.Opacity = 1;
        };

        PART_PopupBorder.BeginAnimation(OpacityProperty, anim);
    }
}
