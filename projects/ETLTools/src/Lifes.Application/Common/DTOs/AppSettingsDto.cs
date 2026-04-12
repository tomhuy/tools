namespace Lifes.Application.Common.DTOs;

/// <summary>
/// Data Transfer Object for application settings.
/// </summary>
public class AppSettingsDto
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
