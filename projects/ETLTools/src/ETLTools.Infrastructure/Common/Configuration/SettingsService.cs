using System.Text.Json;
using ETLTools.Core.Interfaces;
using ETLTools.Core.Models;
using Microsoft.Extensions.Logging;

namespace ETLTools.Infrastructure.Common.Configuration;

/// <summary>
/// Service for persisting application settings to JSON file.
/// </summary>
public class SettingsService : ISettingsService
{
    private readonly ILogger<SettingsService> _logger;
    private readonly string _settingsFilePath;
    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        WriteIndented = true,
        PropertyNameCaseInsensitive = true
    };

    public SettingsService(ILogger<SettingsService> logger)
    {
        _logger = logger;
        
        // Settings file in application directory
        var appDirectory = AppDomain.CurrentDomain.BaseDirectory;
        _settingsFilePath = Path.Combine(appDirectory, "appsettings.user.json");
        
        _logger.LogInformation("SettingsService initialized. Settings file: {FilePath}", _settingsFilePath);
    }

    /// <summary>
    /// Loads application settings from JSON file.
    /// Returns default settings if file not found or invalid.
    /// </summary>
    public async Task<Result<AppSettings>> LoadAsync()
    {
        try
        {
            // Check if file exists
            if (!File.Exists(_settingsFilePath))
            {
                _logger.LogInformation("Settings file not found. Using default settings.");
                return Result<AppSettings>.Success(new AppSettings());
            }

            // Read file
            var json = await File.ReadAllTextAsync(_settingsFilePath);
            
            if (string.IsNullOrWhiteSpace(json))
            {
                _logger.LogWarning("Settings file is empty. Using default settings.");
                return Result<AppSettings>.Success(new AppSettings());
            }

            // Deserialize
            var settings = JsonSerializer.Deserialize<AppSettings>(json, _jsonOptions);
            
            if (settings == null)
            {
                _logger.LogWarning("Failed to deserialize settings. Using default settings.");
                return Result<AppSettings>.Success(new AppSettings());
            }

            _logger.LogInformation("Settings loaded successfully from {FilePath}", _settingsFilePath);
            _logger.LogDebug("Last directory: {LastDirectory}", settings.LastDirectory);
            
            return Result<AppSettings>.Success(settings);
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Invalid JSON in settings file. Using default settings.");
            return Result<AppSettings>.Success(new AppSettings());
        }
        catch (IOException ex)
        {
            _logger.LogError(ex, "IO error loading settings. Using default settings.");
            return Result<AppSettings>.Success(new AppSettings());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error loading settings. Using default settings.");
            return Result<AppSettings>.Success(new AppSettings());
        }
    }

    /// <summary>
    /// Saves application settings to JSON file.
    /// </summary>
    public async Task<Result> SaveAsync(AppSettings settings)
    {
        if (settings == null)
        {
            return Result.Failure("Settings cannot be null");
        }

        try
        {
            // Ensure directory exists
            var directory = Path.GetDirectoryName(_settingsFilePath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            // Serialize to JSON
            var json = JsonSerializer.Serialize(settings, _jsonOptions);

            // Write to file
            await File.WriteAllTextAsync(_settingsFilePath, json);

            _logger.LogInformation("Settings saved successfully to {FilePath}", _settingsFilePath);
            _logger.LogDebug("Last directory saved: {LastDirectory}", settings.LastDirectory);

            return Result.Success();
        }
        catch (IOException ex)
        {
            var errorMessage = $"IO error saving settings: {ex.Message}";
            _logger.LogError(ex, "Failed to save settings");
            return Result.Failure(errorMessage);
        }
        catch (UnauthorizedAccessException ex)
        {
            var errorMessage = $"Access denied saving settings: {ex.Message}";
            _logger.LogError(ex, "Failed to save settings");
            return Result.Failure(errorMessage);
        }
        catch (Exception ex)
        {
            var errorMessage = $"Unexpected error saving settings: {ex.Message}";
            _logger.LogError(ex, "Failed to save settings");
            return Result.Failure(errorMessage);
        }
    }
}
