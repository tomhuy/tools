using System.Windows;
using ETLTools.Application.Common.Commands;
using ETLTools.Application.Features.VersionIncrease.Commands;
using ETLTools.Application.Services;
using ETLTools.Core.Interfaces;
using ETLTools.Core.Models;
using ETLTools.Infrastructure.Common.Configuration;
using ETLTools.Infrastructure.Features.VersionIncrease.Services;
using ETLTools.Presentation.WPF.Constants;
using ETLTools.Presentation.WPF.Features.VersionIncrease;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;

namespace ETLTools.Presentation.WPF;

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
            .WriteTo.File("logs/etltools-.txt", rollingInterval: RollingInterval.Day, retainedFileCountLimit: 30)
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
        services.AddSingleton<IGitService, ETLTools.Infrastructure.Features.VersionIncrease.Git.GitService>();

        // Application Layer - Commands
        services.AddTransient<IScanProjectsCommand,ScanProjectsCommand>();
        services.AddTransient<IUpdateVersionsCommand, UpdateVersionsCommand>();
        services.AddTransient<ILoadSettingsCommand, LoadSettingsCommand>();
        services.AddTransient<ISaveSettingsCommand, SaveSettingsCommand>();
        services.AddTransient<CommitChangesCommand>();

        // Presentation Layer - ViewModels
        services.AddTransient<VersionIncreaseViewModel>();

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
