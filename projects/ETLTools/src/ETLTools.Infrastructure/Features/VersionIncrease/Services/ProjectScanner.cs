using ETLTools.Core.Interfaces;
using ETLTools.Core.Models;
using Microsoft.Extensions.Logging;

namespace ETLTools.Infrastructure.Features.VersionIncrease.Services;

/// <summary>
/// Service for scanning .csproj files in a directory.
/// </summary>
public class ProjectScanner : IProjectScanner
{
    private readonly ILogger<ProjectScanner> _logger;

    public ProjectScanner(ILogger<ProjectScanner> logger)
    {
        _logger = logger;
    }

    /// <inheritdoc/>
    public async Task<Result<IEnumerable<string>>> ScanProjectsAsync(
        string basePath,
        string filePattern = "*ETL.csproj",
        string excludePattern = "Share*")
    {
        try
        {
            if (string.IsNullOrWhiteSpace(basePath))
            {
                return Result<IEnumerable<string>>.Failure("Base path cannot be empty");
            }

            if (!Directory.Exists(basePath))
            {
                return Result<IEnumerable<string>>.Failure($"Directory not found: {basePath}");
            }

            _logger.LogInformation("Scanning projects in directory: {BasePath}", basePath);

            // Use async file enumeration
            var projectFiles = new List<string>();
            await Task.Run(() =>
            {
                var allCsprojFiles = Directory.GetFiles(basePath, "*.csproj", SearchOption.AllDirectories);

                foreach (var file in allCsprojFiles)
                {
                    var fileName = Path.GetFileName(file);
                    var nameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);

                    // Apply filters:
                    // 1. Must end with "ETL" (before extension)
                    if (!nameWithoutExtension.EndsWith("ETL", StringComparison.OrdinalIgnoreCase))
                        continue;

                    // 2. Must NOT start with "Share" (case-insensitive)
                    if (fileName.StartsWith("Share", StringComparison.OrdinalIgnoreCase))
                        continue;

                    projectFiles.Add(file);
                }
            });

            _logger.LogInformation("Found {Count} candidate projects", projectFiles.Count);

            return Result<IEnumerable<string>>.Success(projectFiles);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to scan projects in directory: {BasePath}", basePath);
            return Result<IEnumerable<string>>.Failure($"Failed to scan projects: {ex.Message}");
        }
    }
}
