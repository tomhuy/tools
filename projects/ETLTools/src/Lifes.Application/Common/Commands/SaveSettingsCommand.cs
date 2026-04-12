using Lifes.Application.Common.DTOs;
using Lifes.Core.Interfaces;
using Lifes.Core.Models;
using Microsoft.Extensions.Logging;

namespace Lifes.Application.Common.Commands;

/// <summary>
/// Interface for SaveSettingsCommand.
/// </summary>
public interface ISaveSettingsCommand
{
    Task<Result> ExecuteAsync(AppSettingsDto settingsDto);
}

/// <summary>
/// Command to save application settings.
/// </summary>
public class SaveSettingsCommand : ISaveSettingsCommand
{
    private readonly ISettingsService _settingsService;
    private readonly ILogger<SaveSettingsCommand> _logger;

    public SaveSettingsCommand(
        ISettingsService settingsService,
        ILogger<SaveSettingsCommand> logger)
    {
        _settingsService = settingsService;
        _logger = logger;
    }

    /// <summary>
    /// Executes the save settings command.
    /// </summary>
    /// <param name="settingsDto">The settings to save.</param>
    /// <returns>A result indicating success or failure.</returns>
    public async Task<Result> ExecuteAsync(AppSettingsDto settingsDto)
    {
        if (settingsDto == null)
        {
            return Result.Failure("Settings cannot be null");
        }

        _logger.LogInformation("Saving application settings...");

        try
        {
            // Validate DTO
            if (string.IsNullOrWhiteSpace(settingsDto.LastDirectory))
            {
                return Result.Failure("Last directory cannot be empty");
            }

            // Map to domain model
            var settings = new AppSettings
            {
                LastDirectory = settingsDto.LastDirectory,
                Version = settingsDto.Version
            };

            // Save to persistence
            var saveResult = await _settingsService.SaveAsync(settings);

            if (!saveResult.IsSuccess)
            {
                _logger.LogError("Failed to save settings: {Error}", saveResult.Error);
                return saveResult;
            }

            _logger.LogInformation("Settings saved successfully");
            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error saving settings");
            return Result.Failure($"Unexpected error: {ex.Message}");
        }
    }
}
