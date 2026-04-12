using ETLTools.Core.Models;

namespace ETLTools.Core.Interfaces;

/// <summary>
/// Interface for Git operations.
/// </summary>
public interface IGitService
{
    /// <summary>
    /// Checks if the specified path is a Git repository.
    /// </summary>
    /// <param name="path">The path to check.</param>
    /// <returns>A result indicating whether the path is a Git repository.</returns>
    Task<Result<bool>> IsGitRepositoryAsync(string path);

    /// <summary>
    /// Checks if there are any changes in the repository.
    /// </summary>
    /// <param name="repoPath">The path to the Git repository.</param>
    /// <returns>A result indicating whether there are changes.</returns>
    Task<Result<bool>> HasChangesAsync(string repoPath);

    /// <summary>
    /// Gets the list of modified files in the repository.
    /// </summary>
    /// <param name="repoPath">The path to the Git repository.</param>
    /// <returns>A result containing the list of modified file paths.</returns>
    Task<Result<IEnumerable<string>>> GetModifiedFilesAsync(string repoPath);

    /// <summary>
    /// Gets the current branch name.
    /// </summary>
    /// <param name="repoPath">The path to the Git repository.</param>
    /// <returns>A result containing the current branch name.</returns>
    Task<Result<string>> GetCurrentBranchAsync(string repoPath);

    /// <summary>
    /// Stages the specified files for commit.
    /// </summary>
    /// <param name="repoPath">The path to the Git repository.</param>
    /// <param name="files">The files to stage.</param>
    /// <returns>A result indicating success or failure.</returns>
    Task<Result> StageFilesAsync(string repoPath, IEnumerable<string> files);

    /// <summary>
    /// Creates a commit with the specified message.
    /// </summary>
    /// <param name="repoPath">The path to the Git repository.</param>
    /// <param name="message">The commit message.</param>
    /// <param name="authorName">Optional author name. If null, uses git config.</param>
    /// <param name="authorEmail">Optional author email. If null, uses git config.</param>
    /// <returns>A result containing the commit SHA.</returns>
    Task<Result<string>> CommitAsync(string repoPath, string message, string? authorName = null, string? authorEmail = null);

    /// <summary>
    /// Pushes commits to the remote repository.
    /// </summary>
    /// <param name="repoPath">The path to the Git repository.</param>
    /// <param name="remoteName">The name of the remote (e.g., "origin").</param>
    /// <param name="branchName">The name of the branch to push.</param>
    /// <returns>A result indicating success or failure.</returns>
    Task<Result> PushAsync(string repoPath, string remoteName, string branchName);
}
