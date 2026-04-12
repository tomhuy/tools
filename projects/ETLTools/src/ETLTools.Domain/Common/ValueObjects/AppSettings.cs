using ETLTools.Core.Models;

namespace ETLTools.Domain.Common.ValueObjects;

/// <summary>
/// Value object representing application settings with validation logic.
/// </summary>
public class AppSettings
{
    public string LastDirectory { get; private set; }
    public string Version { get; private set; }

    private AppSettings(string lastDirectory, string version)
    {
        LastDirectory = lastDirectory;
        Version = version;
    }

    /// <summary>
    /// Creates default settings.
    /// </summary>
    public static AppSettings CreateDefault()
    {
        return new AppSettings(
            lastDirectory: Environment.CurrentDirectory,
            version: "1.0"
        );
    }

    /// <summary>
    /// Creates settings with specified last directory.
    /// </summary>
    public static Result<AppSettings> Create(string lastDirectory, string? version = null)
    {
        // Validate last directory
        if (string.IsNullOrWhiteSpace(lastDirectory))
        {
            return Result<AppSettings>.Failure("Last directory cannot be empty");
        }

        // Check if directory format is valid (not necessarily exists)
        try
        {
            var fullPath = Path.GetFullPath(lastDirectory);
        }
        catch (Exception ex)
        {
            return Result<AppSettings>.Failure($"Invalid directory path: {ex.Message}");
        }

        return Result<AppSettings>.Success(new AppSettings(
            lastDirectory: lastDirectory,
            version: version ?? "1.0"
        ));
    }

    /// <summary>
    /// Validates the current settings.
    /// </summary>
    public Result Validate()
    {
        if (string.IsNullOrWhiteSpace(LastDirectory))
        {
            return Result.Failure("Last directory cannot be empty");
        }

        // Note: We don't check if directory exists, as it might be deleted
        // The app should handle non-existent directories gracefully

        return Result.Success();
    }

    /// <summary>
    /// Creates a new AppSettings with updated last directory (immutable).
    /// </summary>
    public Result<AppSettings> WithLastDirectory(string newDirectory)
    {
        return Create(newDirectory, Version);
    }

    /// <summary>
    /// Maps to Core.Interfaces.AppSettings for persistence.
    /// </summary>
    public Core.Interfaces.AppSettings ToPersistenceModel()
    {
        return new Core.Interfaces.AppSettings
        {
            LastDirectory = LastDirectory,
            Version = Version
        };
    }

    /// <summary>
    /// Creates from Core.Interfaces.AppSettings.
    /// </summary>
    public static Result<AppSettings> FromPersistenceModel(Core.Interfaces.AppSettings model)
    {
        if (model == null)
        {
            return Result<AppSettings>.Failure("Settings model cannot be null");
        }

        return Create(model.LastDirectory ?? string.Empty, model.Version);
    }
}
