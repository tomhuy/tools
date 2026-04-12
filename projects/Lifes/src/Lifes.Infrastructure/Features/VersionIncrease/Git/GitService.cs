using Lifes.Core.Interfaces;
using Lifes.Core.Models;
using LibGit2Sharp;
using Microsoft.Extensions.Logging;

namespace Lifes.Infrastructure.Features.VersionIncrease.Git;

/// <summary>
/// Implementation of Git operations using LibGit2Sharp.
/// </summary>
public class GitService : IGitService
{
    private readonly ILogger<GitService> _logger;

    public GitService(ILogger<GitService> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <inheritdoc />
    public Task<Result<bool>> IsGitRepositoryAsync(string path)
    {
        try
        {
            var repoPath = Repository.Discover(path);
            var isRepo = !string.IsNullOrEmpty(repoPath);

            _logger.LogInformation("Checked Git repository at {Path}: {IsRepo}", path, isRepo);

            return Task.FromResult(Result<bool>.Success(isRepo));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking if path is a Git repository: {Path}", path);
            return Task.FromResult(Result<bool>.Failure($"Failed to check Git repository: {ex.Message}"));
        }
    }

    /// <inheritdoc />
    public Task<Result<bool>> HasChangesAsync(string repoPath)
    {
        try
        {
            using var repo = new Repository(repoPath);
            var status = repo.RetrieveStatus();
            var hasChanges = status.IsDirty;

            _logger.LogInformation("Repository has changes: {HasChanges}", hasChanges);

            return Task.FromResult(Result<bool>.Success(hasChanges));
        }
        catch (RepositoryNotFoundException ex)
        {
            _logger.LogError(ex, "Git repository not found at: {RepoPath}", repoPath);
            return Task.FromResult(Result<bool>.Failure("Git repository not found"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking for changes in repository: {RepoPath}", repoPath);
            return Task.FromResult(Result<bool>.Failure($"Failed to check for changes: {ex.Message}"));
        }
    }

    /// <inheritdoc />
    public Task<Result<IEnumerable<string>>> GetModifiedFilesAsync(string repoPath)
    {
        try
        {
            using var repo = new Repository(repoPath);
            var status = repo.RetrieveStatus();

            var modifiedFiles = status
                .Where(s => s.State != FileStatus.Ignored && s.State != FileStatus.Unaltered)
                .Select(s => s.FilePath)
                .ToList();

            _logger.LogInformation("Found {Count} modified files", modifiedFiles.Count);

            return Task.FromResult(Result<IEnumerable<string>>.Success(modifiedFiles));
        }
        catch (RepositoryNotFoundException ex)
        {
            _logger.LogError(ex, "Git repository not found at: {RepoPath}", repoPath);
            return Task.FromResult(Result<IEnumerable<string>>.Failure("Git repository not found"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting modified files: {RepoPath}", repoPath);
            return Task.FromResult(Result<IEnumerable<string>>.Failure($"Failed to get modified files: {ex.Message}"));
        }
    }

    /// <inheritdoc />
    public Task<Result<string>> GetCurrentBranchAsync(string repoPath)
    {
        try
        {
            using var repo = new Repository(repoPath);
            var branchName = repo.Head.FriendlyName;

            _logger.LogInformation("Current branch: {BranchName}", branchName);

            return Task.FromResult(Result<string>.Success(branchName));
        }
        catch (RepositoryNotFoundException ex)
        {
            _logger.LogError(ex, "Git repository not found at: {RepoPath}", repoPath);
            return Task.FromResult(Result<string>.Failure("Git repository not found"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting current branch: {RepoPath}", repoPath);
            return Task.FromResult(Result<string>.Failure($"Failed to get current branch: {ex.Message}"));
        }
    }

    /// <inheritdoc />
    public Task<Result> StageFilesAsync(string repoPath, IEnumerable<string> files)
    {
        try
        {
            using var repo = new Repository(repoPath);
            var filesList = files.ToList();

            foreach (var file in filesList)
            {
                Commands.Stage(repo, file);
                _logger.LogDebug("Staged file: {File}", file);
            }

            _logger.LogInformation("Staged {Count} files", filesList.Count);

            return Task.FromResult(Result.Success());
        }
        catch (RepositoryNotFoundException ex)
        {
            _logger.LogError(ex, "Git repository not found at: {RepoPath}", repoPath);
            return Task.FromResult(Result.Failure("Git repository not found"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error staging files: {RepoPath}", repoPath);
            return Task.FromResult(Result.Failure($"Failed to stage files: {ex.Message}"));
        }
    }

    /// <inheritdoc />
    public Task<Result<string>> CommitAsync(string repoPath, string message, string? authorName = null, string? authorEmail = null)
    {
        try
        {
            using var repo = new Repository(repoPath);

            Signature signature;
            if (!string.IsNullOrEmpty(authorName) && !string.IsNullOrEmpty(authorEmail))
            {
                signature = new Signature(authorName, authorEmail, DateTimeOffset.Now);
            }
            else
            {
                signature = repo.Config.BuildSignature(DateTimeOffset.Now);
            }

            var commit = repo.Commit(message, signature, signature);
            var commitSha = commit.Sha;

            _logger.LogInformation("Created commit: {CommitSha}", commitSha);

            return Task.FromResult(Result<string>.Success(commitSha));
        }
        catch (RepositoryNotFoundException ex)
        {
            _logger.LogError(ex, "Git repository not found at: {RepoPath}", repoPath);
            return Task.FromResult(Result<string>.Failure("Git repository not found"));
        }
        catch (EmptyCommitException ex)
        {
            _logger.LogWarning(ex, "No changes to commit");
            return Task.FromResult(Result<string>.Failure("No changes to commit"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating commit: {RepoPath}", repoPath);
            return Task.FromResult(Result<string>.Failure($"Failed to create commit: {ex.Message}"));
        }
    }

    /// <inheritdoc />
    public Task<Result> PushAsync(string repoPath, string remoteName, string branchName)
    {
        try
        {
            using var repo = new Repository(repoPath);
            var remote = repo.Network.Remotes[remoteName];

            if (remote == null)
            {
                _logger.LogError("Remote not found: {RemoteName}", remoteName);
                return Task.FromResult(Result.Failure($"Remote '{remoteName}' not found"));
            }

            var pushRefSpec = $"refs/heads/{branchName}:refs/heads/{branchName}";
            var pushOptions = new PushOptions
            {
                CredentialsProvider = (url, usernameFromUrl, types) =>
                {
                    return new DefaultCredentials();
                }
            };

            repo.Network.Push(remote, pushRefSpec, pushOptions);

            _logger.LogInformation("Pushed to {RemoteName}/{BranchName}", remoteName, branchName);

            return Task.FromResult(Result.Success());
        }
        catch (RepositoryNotFoundException ex)
        {
            _logger.LogError(ex, "Git repository not found at: {RepoPath}", repoPath);
            return Task.FromResult(Result.Failure("Git repository not found"));
        }
        catch (LibGit2SharpException ex) when (ex.Message.Contains("authentication"))
        {
            _logger.LogError(ex, "Authentication failed during push");
            return Task.FromResult(Result.Failure("Push failed: Authentication required. Please configure Git credentials."));
        }
        catch (LibGit2SharpException ex) when (ex.Message.Contains("network"))
        {
            _logger.LogError(ex, "Network error during push");
            return Task.FromResult(Result.Failure("Push failed: Network error. Please check your connection."));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error pushing to remote: {RemoteName}/{BranchName}", remoteName, branchName);
            return Task.FromResult(Result.Failure($"Failed to push: {ex.Message}"));
        }
    }
}
