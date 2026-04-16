using System.Windows;
using Lifes.Core.Interfaces;
using Lifes.Core.Models;
using Lifes.Presentation.WPF.Features.VersionIncrease;
using Lifes.Presentation.WPF.Features.DashboardChart;
using Lifes.Presentation.WPF.Features.DocumentManagement;
using Lifes.Presentation.WPF.Features.AnnualCalendar;
using Microsoft.Extensions.Logging;

namespace Lifes.Presentation.WPF;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private readonly VersionIncreaseViewModel _versionIncreaseViewModel;
    private readonly DashboardChartViewModel _dashboardChartViewModel;
    private readonly DocumentManagementViewModel _documentManagementViewModel;
    private readonly AnnualCalendarViewModel _annualCalendarViewModel;
    private readonly MonthlyCalendarViewModel _monthlyCalendarViewModel;
    private readonly INavigationService _navigationService;
    private readonly ILogger<MainWindow> _logger;

    public MainWindow(
        VersionIncreaseViewModel versionIncreaseViewModel,
        DashboardChartViewModel dashboardChartViewModel,
        DocumentManagementViewModel documentManagementViewModel,
        AnnualCalendarViewModel annualCalendarViewModel,
        MonthlyCalendarViewModel monthlyCalendarViewModel,
        INavigationService navigationService,
        ILogger<MainWindow> logger)
    {
        InitializeComponent();

        _versionIncreaseViewModel = versionIncreaseViewModel;
        _dashboardChartViewModel = dashboardChartViewModel;
        _documentManagementViewModel = documentManagementViewModel;
        _annualCalendarViewModel = annualCalendarViewModel;
        _monthlyCalendarViewModel = monthlyCalendarViewModel;
        _navigationService = navigationService;
        _logger = logger;

        // Subscribe before showing any view so no navigation event is missed
        _navigationService.ToolNavigated += OnToolNavigated;

        // Show the initial Dashboard view
        ShowDashboardChartView();

        _logger.LogInformation("Main window initialized with Dashboard Chart Tool");
    }

    private void ShowVersionIncreaseView()
    {
        var view = new VersionIncreaseView
        {
            DataContext = _versionIncreaseViewModel
        };
        MainContentControl.Content = view;
    }

    private void ShowDashboardChartView()
    {
        var view = new DashboardChartView
        {
            DataContext = _dashboardChartViewModel
        };
        MainContentControl.Content = view;
    }

    private void ShowDocumentManagementView()
    {
        var view = new DocumentManagementView
        {
            DataContext = _documentManagementViewModel
        };
        MainContentControl.Content = view;
    }

    private void ShowMonthlyCalendarView()
    {
        var view = new MonthlyCalendarView
        {
            DataContext = _monthlyCalendarViewModel
        };
        MainContentControl.Content = view;
    }

    private void ShowAnnualCalendarView()
    {
        var view = new AnnualCalendarView
        {
            DataContext = _annualCalendarViewModel
        };
        MainContentControl.Content = view;
    }

    /// <summary>
    /// Swaps the main content area when the user navigates to a different tool.
    /// </summary>
    private void OnToolNavigated(object? sender, ToolNavigatedEventArgs e)
    {
        _logger.LogInformation("Navigating to tool: {ToolId}", e.Tool.Id);

        Dispatcher.Invoke(() =>
        {
            switch (e.Tool.Id)
            {
                case Constants.ToolIds.VersionIncrease:
                    ShowVersionIncreaseView();
                    break;

                case Constants.ToolIds.DashboardChart:
                    ShowDashboardChartView();
                    break;

                case Constants.ToolIds.DocumentManagement:
                    ShowDocumentManagementView();
                    break;

                case Constants.ToolIds.AnnualCalendar:
                    ShowAnnualCalendarView();
                    break;

                case Constants.ToolIds.MonthlyCalendar:
                    ShowMonthlyCalendarView();
                    break;

                default:
                    _logger.LogWarning("No view registered for tool '{ToolId}'", e.Tool.Id);
                    break;
            }
        });
    }
}
