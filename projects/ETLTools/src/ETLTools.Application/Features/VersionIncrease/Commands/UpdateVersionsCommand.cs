using ETLTools.Application.Features.VersionIncrease.DTOs;
using ETLTools.Core.Interfaces;
using ETLTools.Core.Models;
using Microsoft.Extensions.Logging;

namespace ETLTools.Application.Features.VersionIncrease.Commands;

/// <summary>
/// Interface for UpdateVersionsCommand.
/// </summary>
public interface IUpdateVersionsCommand
{
    Task<Result<VersionUpdateResultDto>> ExecuteAsync(
        IEnumerable<ProjectFileDto> projects,
        DateTime? targetDate = null,
        IProgress<(int current, int total)>? progress = null);
}

/// <summary>
/// Command to update versions for multiple projects.
/// </summary>
public class UpdateVersionsCommand : IUpdateVersionsCommand
{
    private readonly IProjectFileService _projectFileService;
    private readonly IVersionService _versionService;
    private readonly ILogger<UpdateVersionsCommand> _logger;

    public UpdateVersionsCommand(
        IProjectFileService projectFileService,
        IVersionService versionService,
        ILogger<UpdateVersionsCommand> logger)
    {
        _projectFileService = projectFileService;
        _versionService = versionService;
        _logger = logger;
    }

    /// <summary>
    /// Executes the update versions command.
    /// </summary>
    /// <param name="projects">List of projects to update.</param>
    /// <param name="targetDate">Target date for version increment (default: today).</param>
    /// <param name="progress">Progress reporter for UI updates.</param>
    /// <returns>Result containing update summary.</returns>
    public async Task<Result<VersionUpdateResultDto>> ExecuteAsync(
        IEnumerable<ProjectFileDto> projects,
        DateTime? targetDate = null,
        IProgress<(int current, int total)>? progress = null)
    {
        try
        {
            var projectsList = projects.ToList();
            var date = targetDate ?? DateTime.Now;
            var result = new VersionUpdateResultDto
            {
                TotalProjects = projectsList.Count
            };

            _logger.LogInformation("Updating versions for {Count} projects to date {Date}", 
                projectsList.Count, date.ToString("yyyy-MM-dd"));

            int current = 0;
            foreach (var project in projectsList)
            {
                current++;
                progress?.Report((current, projectsList.Count));

                var updateResult = await UpdateSingleProjectAsync(project, date);
                result.Updates.Add(updateResult);

                if (updateResult.Success)
                {
                    result.SuccessCount++;
                }
                else
                {
                    result.FailedCount++;
                }
            }

            _logger.LogInformation(result.Summary);

            return Result<VersionUpdateResultDto>.Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update versions");
            return Result<VersionUpdateResultDto>.Failure($"Failed to update versions: {ex.Message}");
        }
    }

    private async Task<ProjectUpdateDto> UpdateSingleProjectAsync(ProjectFileDto project, DateTime targetDate)
    {
        var updateDto = new ProjectUpdateDto
        {
            ProjectName = project.FileName,
            OldVersion = project.CurrentVersion
        };

        try
        {
            // Parse current version
            var parseResult = _versionService.ParseVersion(project.CurrentVersion);
            if (!parseResult.IsSuccess)
            {
                updateDto.Success = false;
                updateDto.ErrorMessage = parseResult.Error;
                _logger.LogWarning("Failed to parse version for {ProjectName}: {Error}", 
                    project.FileName, parseResult.Error);
                return updateDto;
            }

            // Increment version
            var incrementResult = _versionService.IncrementVersion(parseResult.Value!, targetDate);
            if (!incrementResult.IsSuccess)
            {
                updateDto.Success = false;
                updateDto.ErrorMessage = incrementResult.Error;
                _logger.LogWarning("Failed to increment version for {ProjectName}: {Error}", 
                    project.FileName, incrementResult.Error);
                return updateDto;
            }

            // Format new version
            var formatResult = _versionService.FormatVersion(incrementResult.Value!);
            if (!formatResult.IsSuccess)
            {
                updateDto.Success = false;
                updateDto.ErrorMessage = formatResult.Error;
                return updateDto;
            }

            var newVersion = formatResult.Value!;
            updateDto.NewVersion = newVersion;

            // Update file
            var updateFileResult = await _projectFileService.UpdateVersionAsync(project.FullPath, newVersion);
            if (!updateFileResult.IsSuccess)
            {
                updateDto.Success = false;
                updateDto.ErrorMessage = updateFileResult.Error;
                _logger.LogError("Failed to update file {ProjectName}: {Error}", 
                    project.FileName, updateFileResult.Error);
                return updateDto;
            }

            updateDto.Success = true;
            _logger.LogInformation("Updated {ProjectName}: {OldVersion} → {NewVersion}", 
                project.FileName, project.CurrentVersion, newVersion);

            return updateDto;
        }
        catch (Exception ex)
        {
            updateDto.Success = false;
            updateDto.ErrorMessage = ex.Message;
            _logger.LogError(ex, "Unexpected error updating {ProjectName}", project.FileName);
            return updateDto;
        }
    }
}
