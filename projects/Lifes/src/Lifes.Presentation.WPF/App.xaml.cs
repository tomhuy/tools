using System.Windows;
using Lifes.Application.Common.Commands;
using Lifes.Application.Features.DashboardChart.Implements;
using Lifes.Application.Features.VersionIncrease.Commands;
using Lifes.Application.Services;
using Lifes.Core.Interfaces;
using Lifes.Core.Models;
using Lifes.Infrastructure.Common.Configuration;
using Lifes.Infrastructure.Features.VersionIncrease.Services;
using Lifes.Presentation.WPF.Constants;
using Lifes.Presentation.WPF.Features.VersionIncrease;
using Lifes.Application.Features.DashboardChart.Interfaces;
using Lifes.Presentation.WPF.Features.DashboardChart;
using Lifes.Presentation.WPF.Features.DocumentManagement;
using Lifes.Infrastructure.Features.DocumentManagement.Services;
using Lifes.Presentation.WPF.Features.AnnualCalendar;
using Lifes.Infrastructure.Features.AnnualCalendar.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;

namespace Lifes.Presentation.WPF;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : System.Windows.Application
{
    private ServiceProvider? _serviceProvider;

    /// <summary>
    /// Application startup event handler.
    /// </summary>
    private void Application_Startup(object sender, StartupEventArgs e)
    {
        // Configure Serilog
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.Console()
            .WriteTo.File("logs/Lifes-.txt", rollingInterval: RollingInterval.Day, retainedFileCountLimit: 30)
            .CreateLogger();

        Log.Information("ETL Tools Application Starting");

        // Setup Dependency Injection
        var serviceCollection = new ServiceCollection();
        ConfigureServices(serviceCollection);
        _serviceProvider = serviceCollection.BuildServiceProvider();

        // Register tools before the first ViewModel is created so that
        // GetAllTools() returns a complete list during ViewModel initialisation.
        RegisterNavigationTools(_serviceProvider);

        // Create and show main window
        var mainWindow = _serviceProvider.GetRequiredService<MainWindow>();
        mainWindow.Show();
    }

    /// <summary>
    /// Configures dependency injection services.
    /// </summary>
    private void ConfigureServices(IServiceCollection services)
    {
        // Logging
        services.AddLogging(builder =>
        {
            builder.ClearProviders();
            builder.AddSerilog(dispose: true);
        });

        // Navigation service (singleton so all consumers share the same state)
        services.AddSingleton<INavigationService, NavigationService>();

        // Core Interfaces -> Infrastructure Implementations
        services.AddSingleton<IProjectScanner, ProjectScanner>();
        services.AddSingleton<IProjectFileService, ProjectFileService>();
        services.AddSingleton<IVersionService, VersionService>();
        services.AddSingleton<ISettingsService, SettingsService>();
        services.AddSingleton<IGitService, Lifes.Infrastructure.Features.VersionIncrease.Git.GitService>();

        // Application Layer - Commands
        services.AddTransient<IScanProjectsCommand,ScanProjectsCommand>();
        services.AddTransient<IUpdateVersionsCommand, UpdateVersionsCommand>();
        services.AddTransient<ILoadSettingsCommand, LoadSettingsCommand>();
        services.AddTransient<ISaveSettingsCommand, SaveSettingsCommand>();
        services.AddTransient<CommitChangesCommand>();

        // Presentation Layer - ViewModels
        services.AddTransient<VersionIncreaseViewModel>();
        services.AddTransient<DashboardChartViewModel>();
        services.AddTransient<DocumentManagementViewModel>();
        services.AddTransient<AnnualCalendarViewModel>();
        services.AddTransient<MonthlyCalendarViewModel>();

        // Dashboard Services
        services.AddSingleton<IDashboardDataService, MockDashboardDataService>();
        services.AddSingleton<IDocumentService, MockDocumentService>();
        services.AddSingleton<ICalendarService, MockCalendarService>();

        // Presentation Layer - Views & Windows
        services.AddSingleton<MainWindow>();
    }

    /// <summary>
    /// Registers all available tools so the navigation menu is populated.
    /// Add new tools here as they are implemented.
    /// </summary>
    private static void RegisterNavigationTools(ServiceProvider serviceProvider)
    {
        var nav = serviceProvider.GetRequiredService<INavigationService>();

        nav.RegisterTool(new ToolDefinition
        {
            Id = ToolIds.VersionIncrease,
            Name = "Version Increase Tool",
            Description = "Increment version numbers for ETL projects"
        });

        nav.RegisterTool(new ToolDefinition
        {
            Id = ToolIds.DashboardChart,
            Name = "Dashboard Chart",
            Description = "View dynamic dashboard chart layout"
        });

        nav.RegisterTool(new ToolDefinition
        {
            Id = ToolIds.DocumentManagement,
            Name = "Document Management",
            Description = "Tracker Grid for Document Management"
        });

        nav.RegisterTool(new ToolDefinition
        {
            Id = ToolIds.AnnualCalendar,
            Name = "Annual Linear Calendar",
            Description = "View yearly events on a beautiful dynamic gantt chart"
        });

        nav.RegisterTool(new ToolDefinition
        {
            Id = ToolIds.MonthlyCalendar,
            Name = "Monthly Calendar",
            Description = "Traditional grid view with colorful event bars"
        });
    }

    /// <summary>
    /// Application exit event handler.
    /// </summary>
    protected override void OnExit(ExitEventArgs e)
    {
        Log.Information("ETL Tools Application Exiting");
        Log.CloseAndFlush();
        
        _serviceProvider?.Dispose();
        base.OnExit(e);
    }
}
