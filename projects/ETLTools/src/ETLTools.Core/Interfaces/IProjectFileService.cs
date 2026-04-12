using ETLTools.Core.Models;

namespace ETLTools.Core.Interfaces;

/// <summary>
/// Interface for reading and updating .csproj files.
/// </summary>
public interface IProjectFileService
{
    /// <summary>
    /// Reads the version from a .csproj file.
    /// </summary>
    /// <param name="filePath">The full path to the .csproj file.</param>
    /// <returns>A result containing the version string.</returns>
    Task<Result<string>> ReadVersionAsync(string filePath);

    /// <summary>
    /// Updates the version in a .csproj file.
    /// </summary>
    /// <param name="filePath">The full path to the .csproj file.</param>
    /// <param name="newVersion">The new version string to set.</param>
    /// <returns>A result indicating success or failure.</returns>
    Task<Result> UpdateVersionAsync(string filePath, string newVersion);
}
