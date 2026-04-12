using ETLTools.Core.Models;

namespace ETLTools.Core.Interfaces;

/// <summary>
/// Interface for managing application settings persistence.
/// </summary>
public interface ISettingsService
{
    /// <summary>
    /// Loads application settings from storage.
    /// Returns default settings if file not found or invalid.
    /// </summary>
    /// <returns>A result containing the loaded AppSettings.</returns>
    Task<Result<AppSettings>> LoadAsync();

    /// <summary>
    /// Saves application settings to storage.
    /// </summary>
    /// <param name="settings">The settings to save.</param>
    /// <returns>A result indicating success or failure.</returns>
    Task<Result> SaveAsync(AppSettings settings);
}

/// <summary>
/// Represents application settings.
/// </summary>
public class AppSettings
{
    /// <summary>
    /// Last directory used by the user.
    /// </summary>
    public string LastDirectory { get; set; } = string.Empty;

    /// <summary>
    /// Settings file format version.
    /// </summary>
    public string Version { get; set; } = "1.0";
}
