using ETLTools.Application.Features.VersionIncrease.DTOs;
using ETLTools.Core.Interfaces;
using ETLTools.Core.Models;
using Microsoft.Extensions.Logging;

namespace ETLTools.Application.Features.VersionIncrease.Commands;

/// <summary>
/// Command for committing and pushing changes to Git repository.
/// </summary>
public class CommitChangesCommand
{
    private readonly IGitService _gitService;
    private readonly ILogger<CommitChangesCommand> _logger;

    public CommitChangesCommand(IGitService gitService, ILogger<CommitChangesCommand> logger)
    {
        _gitService = gitService ?? throw new ArgumentNullException(nameof(gitService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Executes the commit and push workflow.
    /// </summary>
    /// <param name="dto">The commit changes request data.</param>
    /// <param name="progress">Optional progress reporter.</param>
    /// <returns>A result containing the commit operation details.</returns>
    public async Task<Result<CommitResultDto>> ExecuteAsync(
        CommitChangesDto dto,
        IProgress<string>? progress = null)
    {
        try
        {
            _logger.LogInformation("Starting commit and push workflow for repository: {RepositoryPath}", dto.RepositoryPath);

            var result = new CommitResultDto();

            // Step 1: Validate Git repository
            progress?.Report("Validating Git repository...");
            var isRepoResult = await _gitService.IsGitRepositoryAsync(dto.RepositoryPath);
            if (!isRepoResult.IsSuccess || !isRepoResult.Value)
            {
                result.ErrorMessage = "Current directory is not a Git repository";
                _logger.LogWarning("Not a Git repository: {RepositoryPath}", dto.RepositoryPath);
                return Result<CommitResultDto>.Failure(result.ErrorMessage);
            }

            // Step 2: Get current branch if not specified
            if (string.IsNullOrEmpty(dto.BranchName))
            {
                progress?.Report("Getting current branch...");
                var branchResult = await _gitService.GetCurrentBranchAsync(dto.RepositoryPath);
                if (!branchResult.IsSuccess)
                {
                    result.ErrorMessage = branchResult.Error;
                    return Result<CommitResultDto>.Failure(result.ErrorMessage);
                }
                dto.BranchName = branchResult.Value;
            }
            result.BranchName = dto.BranchName;

            // Step 3: Stage modified files
            progress?.Report($"Staging {dto.ModifiedFiles.Count()} files...");
            var stageResult = await _gitService.StageFilesAsync(dto.RepositoryPath, dto.ModifiedFiles);
            if (!stageResult.IsSuccess)
            {
                result.ErrorMessage = stageResult.Error;
                _logger.LogError("Failed to stage files: {Error}", stageResult.Error);
                return Result<CommitResultDto>.Failure(result.ErrorMessage);
            }

            result.FilesCommitted = dto.ModifiedFiles.Count();

            // Step 4: Create commit
            progress?.Report("Creating commit...");
            var commitResult = await _gitService.CommitAsync(dto.RepositoryPath, dto.CommitMessage);
            if (!commitResult.IsSuccess)
            {
                result.ErrorMessage = commitResult.Error;
                _logger.LogError("Failed to create commit: {Error}", commitResult.Error);
                return Result<CommitResultDto>.Failure(result.ErrorMessage);
            }

            result.CommitSuccess = true;
            result.CommitSha = commitResult.Value;
            _logger.LogInformation("Commit created successfully: {CommitSha}", result.CommitSha);

            // Step 5: Push to remote (if requested)
            if (dto.PushToRemote)
            {
                progress?.Report($"Pushing to {dto.RemoteName}/{dto.BranchName}...");
                var pushResult = await _gitService.PushAsync(dto.RepositoryPath, dto.RemoteName, dto.BranchName);

                if (!pushResult.IsSuccess)
                {
                    result.PushSuccess = false;
                    result.ErrorMessage = pushResult.Error;
                    _logger.LogWarning("Commit succeeded but push failed: {Error}", pushResult.Error);

                    // Partial success - commit succeeded but push failed
                    return Result<CommitResultDto>.Success(result);
                }

                result.PushSuccess = true;
                _logger.LogInformation("Push successful to {RemoteName}/{BranchName}", dto.RemoteName, dto.BranchName);
            }
            else
            {
                result.PushSuccess = false;
                _logger.LogInformation("Push skipped as per user request");
            }

            progress?.Report("Commit and push completed successfully");
            return Result<CommitResultDto>.Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error during commit and push workflow");
            return Result<CommitResultDto>.Failure($"Unexpected error: {ex.Message}");
        }
    }
}
