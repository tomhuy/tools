using Lifes.Domain.Features.VersionIncrease.Enums;
using Lifes.Domain.Features.VersionIncrease.ValueObjects;

namespace Lifes.Domain.Features.VersionIncrease.Entities;

/// <summary>
/// Domain entity representing a project file.
/// </summary>
public class ProjectFile
{
    public string FileName { get; init; } = string.Empty;
    public string FullPath { get; init; } = string.Empty;
    public string RelativePath { get; init; } = string.Empty;
    public VersionInfo? CurrentVersion { get; set; }
    public ProjectStatus Status { get; set; } = ProjectStatus.Ready;

    /// <summary>
    /// Checks if the project file matches the filter criteria.
    /// Business Rules:
    /// - File name must end with "ETL" or "ETL.csproj"
    /// - File extension must be .csproj
    /// - File name must NOT start with "Share" (case-insensitive)
    /// </summary>
    public bool MatchesFilter()
    {
        if (string.IsNullOrWhiteSpace(FileName))
            return false;

        // Check extension
        if (!FileName.EndsWith(".csproj", StringComparison.OrdinalIgnoreCase))
            return false;

        // Check if starts with "Share" (case-insensitive)
        if (FileName.StartsWith("Share", StringComparison.OrdinalIgnoreCase))
            return false;

        // Check if ends with "ETL" before extension
        var nameWithoutExtension = Path.GetFileNameWithoutExtension(FileName);
        if (!nameWithoutExtension.EndsWith("ETL", StringComparison.OrdinalIgnoreCase))
            return false;

        return true;
    }
}
