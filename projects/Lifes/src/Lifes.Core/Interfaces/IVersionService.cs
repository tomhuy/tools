using Lifes.Core.Models;

namespace Lifes.Core.Interfaces;

/// <summary>
/// Interface for version parsing and increment operations.
/// </summary>
public interface IVersionService
{
    /// <summary>
    /// Parses a version string into a VersionInfo object.
    /// </summary>
    /// <param name="versionString">Version string in format yyyy.M.d.{n}.</param>
    /// <returns>A result containing the parsed VersionInfo.</returns>
    Result<VersionInfo> ParseVersion(string versionString);

    /// <summary>
    /// Formats a VersionInfo object to a version string.
    /// </summary>
    /// <param name="version">The VersionInfo to format.</param>
    /// <returns>A result containing the formatted version string.</returns>
    Result<string> FormatVersion(VersionInfo version);

    /// <summary>
    /// Increments a version based on the target date.
    /// </summary>
    /// <param name="current">The current version.</param>
    /// <param name="targetDate">The target date to increment to.</param>
    /// <returns>A result containing the incremented VersionInfo.</returns>
    Result<VersionInfo> IncrementVersion(VersionInfo current, DateTime targetDate);
}

/// <summary>
/// Represents version information in yyyy.M.d.{build} format.
/// </summary>
public class VersionInfo
{
    public int Year { get; set; }
    public int Month { get; set; }
    public int Day { get; set; }
    public int Build { get; set; }

    public DateTime Date => new DateTime(Year, Month, Day);

    public override string ToString() => $"{Year}.{Month}.{Day}.{Build}";
}
