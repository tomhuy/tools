using ETLTools.Application.Common.DTOs;
using ETLTools.Core.Interfaces;
using ETLTools.Core.Models;
using Microsoft.Extensions.Logging;

namespace ETLTools.Application.Common.Commands;

/// <summary>
/// Interface for LoadSettingsCommand.
/// </summary>
public interface ILoadSettingsCommand
{
    Task<Result<AppSettingsDto>> ExecuteAsync();
}

/// <summary>
/// Command to load application settings.
/// </summary>
public class LoadSettingsCommand : ILoadSettingsCommand
{
    private readonly ISettingsService _settingsService;
    private readonly ILogger<LoadSettingsCommand> _logger;

    public LoadSettingsCommand(
        ISettingsService settingsService,
        ILogger<LoadSettingsCommand> logger)
    {
        _settingsService = settingsService;
        _logger = logger;
    }

    /// <summary>
    /// Executes the load settings command.
    /// </summary>
    /// <returns>A result containing the loaded settings DTO.</returns>
    public async Task<Result<AppSettingsDto>> ExecuteAsync()
    {
        _logger.LogInformation("Loading application settings...");

        try
        {
            // Load from persistence
            var loadResult = await _settingsService.LoadAsync();

            if (!loadResult.IsSuccess)
            {
                _logger.LogWarning("Failed to load settings: {Error}", loadResult.Error);
                // Return default settings on failure
                return Result<AppSettingsDto>.Success(new AppSettingsDto());
            }

            var settings = loadResult.Value ?? new Core.Interfaces.AppSettings();

            // Map to DTO
            var dto = new AppSettingsDto
            {
                LastDirectory = settings.LastDirectory ?? string.Empty,
                Version = settings.Version ?? "1.0"
            };

            _logger.LogInformation("Settings loaded successfully");
            return Result<AppSettingsDto>.Success(dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error loading settings");
            // Return default settings on exception
            return Result<AppSettingsDto>.Success(new AppSettingsDto());
        }
    }
}
