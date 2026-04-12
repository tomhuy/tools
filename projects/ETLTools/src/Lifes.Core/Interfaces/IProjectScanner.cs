using Lifes.Core.Models;

namespace Lifes.Core.Interfaces;

/// <summary>
/// Interface for scanning project files in a directory.
/// </summary>
public interface IProjectScanner
{
    /// <summary>
    /// Scans for project files matching the specified patterns.
    /// </summary>
    /// <param name="basePath">The base directory path to scan.</param>
    /// <param name="filePattern">File pattern to match (default: "*ETL.csproj").</param>
    /// <param name="excludePattern">Pattern to exclude files (default: "Share*").</param>
    /// <returns>A result containing the list of project file paths found.</returns>
    Task<Result<IEnumerable<string>>> ScanProjectsAsync(
        string basePath,
        string filePattern = "*ETL.csproj",
        string excludePattern = "Share*");
}
