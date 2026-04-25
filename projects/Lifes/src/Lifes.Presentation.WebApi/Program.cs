using Lifes.Application.Features.VersionIncrease.Commands;
using Lifes.Core.Interfaces;
using Lifes.Infrastructure.Common.Configuration;
using Lifes.Infrastructure.Features.VersionIncrease.Services;
using Lifes.Infrastructure.Features.AnnualCalendar.Repositories;
using Lifes.Infrastructure.Features.AnnualCalendar.Services;
using Lifes.Core.Models;
using Serilog;

// Đảm bảo thư mục làm việc (Working Directory) luôn là thư mục chứa file exe
Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);

var builder = WebApplication.CreateBuilder(args);

// Configure Configuration to require environment-specific appsettings
var envName = builder.Environment.EnvironmentName;
builder.Configuration
    .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{envName}.json", optional: false, reloadOnChange: true)
    .AddEnvironmentVariables();

// Setup Serilog
var logPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs", "backend-.log");
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File(logPath, rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

try
{
    Log.Information("Starting WebApi host in {Environment} mode...", envName);

    // 1. Add CORS to allow Electron/Angular frontend (usually localhost:4200) to connect
    builder.Services.AddCors(options =>
    {
        options.AddPolicy("ElectronVuePolicy",
            policy =>
            {
                policy.AllowAnyOrigin()
                      .AllowAnyHeader()
                      .AllowAnyMethod();
            });
    });

    // 2. Register Application & Infrastructure Services (similar to WPF's App.xaml.cs)
    builder.Services.AddSingleton<IProjectScanner, ProjectScanner>();
    builder.Services.AddSingleton<IProjectFileService, ProjectFileService>();
    builder.Services.AddSingleton<IVersionService, VersionService>();
    builder.Services.AddSingleton<ISettingsService, SettingsService>();
    builder.Services.AddSingleton<IGitService, Lifes.Infrastructure.Features.VersionIncrease.Git.GitService>();

    // Memento & Calendar Services
    builder.Services.AddSingleton<ITagRepository, JsonTagRepository>();
    builder.Services.AddSingleton<IMementoRepository, JsonMementoRepository>();
    builder.Services.AddSingleton<ICalendarService, CalendarService>();

    builder.Services.AddTransient<IScanProjectsCommand, ScanProjectsCommand>();
    builder.Services.AddTransient<IUpdateVersionsCommand, UpdateVersionsCommand>();

    // 3. Add Controllers
    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    var app = builder.Build();

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseCors("ElectronVuePolicy");
    // app.UseHttpsRedirection();
    app.MapControllers();

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Host terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}
