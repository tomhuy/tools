using Lifes.Application.Features.VersionIncrease.DTOs;
using Lifes.Core.Interfaces;
using Lifes.Core.Models;
using Microsoft.Extensions.Logging;

namespace Lifes.Application.Features.VersionIncrease.Commands;

/// <summary>
/// Interface for ScanProjectsCommand.
/// </summary>
public interface IScanProjectsCommand
{
    Task<Result<IEnumerable<ProjectFileDto>>> ExecuteAsync(string basePath);
}

/// <summary>
/// Command to scan for project files and read their versions.
/// </summary>
public class ScanProjectsCommand : IScanProjectsCommand
{
    private readonly IProjectScanner _projectScanner;
    private readonly IProjectFileService _projectFileService;
    private readonly ILogger<ScanProjectsCommand> _logger;

    public ScanProjectsCommand(
        IProjectScanner projectScanner,
        IProjectFileService projectFileService,
        ILogger<ScanProjectsCommand> logger)
    {
        _projectScanner = projectScanner;
        _projectFileService = projectFileService;
        _logger = logger;
    }

    /// <summary>
    /// Executes the scan projects command.
    /// </summary>
    /// <param name="basePath">The base directory to scan.</param>
    /// <returns>Result containing list of project file DTOs.</returns>
    public async Task<Result<IEnumerable<ProjectFileDto>>> ExecuteAsync(string basePath)
    {
        try
        {
            // Step 1: Scan for project files
            var scanResult = await _projectScanner.ScanProjectsAsync(basePath);
            
            if (!scanResult.IsSuccess)
            {
                return Result<IEnumerable<ProjectFileDto>>.Failure(scanResult.Error);
            }

            var projectFiles = scanResult.Value ?? Enumerable.Empty<string>();

            if (!projectFiles.Any())
            {
                _logger.LogWarning("No project files found in {BasePath}", basePath);
                return Result<IEnumerable<ProjectFileDto>>.Success(Enumerable.Empty<ProjectFileDto>());
            }

            // Step 2: Read versions from each project file
            var projectDtos = new List<ProjectFileDto>();

            foreach (var filePath in projectFiles)
            {
                var versionResult = await _projectFileService.ReadVersionAsync(filePath);
                
                var dto = new ProjectFileDto
                {
                    FileName = Path.GetFileName(filePath),
                    FullPath = filePath,
                    RelativePath = GetRelativePath(basePath, filePath),
                    CurrentVersion = versionResult.IsSuccess ? versionResult.Value ?? "Unknown" : "Unknown"
                };

                projectDtos.Add(dto);
            }

            _logger.LogInformation("Successfully loaded {Count} projects", projectDtos.Count);

            return Result<IEnumerable<ProjectFileDto>>.Success(projectDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to scan projects in {BasePath}", basePath);
            return Result<IEnumerable<ProjectFileDto>>.Failure($"Failed to scan projects: {ex.Message}");
        }
    }

    private static string GetRelativePath(string basePath, string fullPath)
    {
        try
        {
            var baseUri = new Uri(basePath.EndsWith(Path.DirectorySeparatorChar.ToString()) 
                ? basePath 
                : basePath + Path.DirectorySeparatorChar);
            var fullUri = new Uri(fullPath);
            var relativeUri = baseUri.MakeRelativeUri(fullUri);
            return Uri.UnescapeDataString(relativeUri.ToString()).Replace('/', Path.DirectorySeparatorChar);
        }
        catch
        {
            return fullPath;
        }
    }
}
