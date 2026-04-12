using Lifes.Core.Interfaces;
using Lifes.Core.Models;
using Microsoft.Extensions.Logging;
using DomainVersionInfo = Lifes.Domain.Features.VersionIncrease.ValueObjects.VersionInfo;

namespace Lifes.Infrastructure.Features.VersionIncrease.Services;

/// <summary>
/// Service for version parsing and increment operations.
/// </summary>
public class VersionService : IVersionService
{
    private readonly ILogger<VersionService> _logger;

    public VersionService(ILogger<VersionService> logger)
    {
        _logger = logger;
    }

    /// <inheritdoc/>
    public Result<VersionInfo> ParseVersion(string versionString)
    {
        try
        {
            var domainVersion = DomainVersionInfo.Parse(versionString);
            
            if (domainVersion == null)
            {
                return Result<VersionInfo>.Failure($"Invalid version format: {versionString}. Expected format: yyyy.M.d.{{n}}");
            }

            var versionInfo = new VersionInfo
            {
                Year = domainVersion.Year,
                Month = domainVersion.Month,
                Day = domainVersion.Day,
                Build = domainVersion.Build
            };

            return Result<VersionInfo>.Success(versionInfo);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to parse version: {VersionString}", versionString);
            return Result<VersionInfo>.Failure($"Failed to parse version: {ex.Message}");
        }
    }

    /// <inheritdoc/>
    public Result<string> FormatVersion(VersionInfo version)
    {
        try
        {
            if (version == null)
            {
                return Result<string>.Failure("Version cannot be null");
            }

            var formatted = $"{version.Year}.{version.Month}.{version.Day}.{version.Build}";
            return Result<string>.Success(formatted);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to format version");
            return Result<string>.Failure($"Failed to format version: {ex.Message}");
        }
    }

    /// <inheritdoc/>
    public Result<VersionInfo> IncrementVersion(VersionInfo current, DateTime targetDate)
    {
        try
        {
            if (current == null)
            {
                return Result<VersionInfo>.Failure("Current version cannot be null");
            }

            // Convert to domain VersionInfo
            var domainCurrent = new DomainVersionInfo
            {
                Year = current.Year,
                Month = current.Month,
                Day = current.Day,
                Build = current.Build
            };

            // Use domain logic to increment
            var incremented = domainCurrent.Increment(targetDate);

            // Convert back to interface VersionInfo
            var result = new VersionInfo
            {
                Year = incremented.Year,
                Month = incremented.Month,
                Day = incremented.Day,
                Build = incremented.Build
            };

            _logger.LogDebug("Incremented version: {OldVersion} → {NewVersion}", 
                current.ToString(), result.ToString());

            return Result<VersionInfo>.Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to increment version");
            return Result<VersionInfo>.Failure($"Failed to increment version: {ex.Message}");
        }
    }
}
