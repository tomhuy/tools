namespace Lifes.Application.Features.VersionIncrease.DTOs;

/// <summary>
/// Data transfer object for commit operation result.
/// </summary>
public class CommitResultDto
{
    /// <summary>
    /// Gets or sets a value indicating whether the commit was successful.
    /// </summary>
    public bool CommitSuccess { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the push was successful.
    /// </summary>
    public bool PushSuccess { get; set; }

    /// <summary>
    /// Gets or sets the commit SHA.
    /// </summary>
    public string CommitSha { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the branch name.
    /// </summary>
    public string BranchName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the number of files committed.
    /// </summary>
    public int FilesCommitted { get; set; }

    /// <summary>
    /// Gets or sets the error message if any operation failed.
    /// </summary>
    public string ErrorMessage { get; set; } = string.Empty;

    /// <summary>
    /// Gets a value indicating whether the overall operation was successful.
    /// </summary>
    public bool IsSuccess => CommitSuccess && (PushSuccess || !PushSuccess && string.IsNullOrEmpty(ErrorMessage));

    /// <summary>
    /// Gets a summary message of the operation.
    /// </summary>
    public string Summary
    {
        get
        {
            if (CommitSuccess && PushSuccess)
            {
                return $"✅ Successfully committed {FilesCommitted} files and pushed to {BranchName}";
            }
            else if (CommitSuccess && !PushSuccess)
            {
                return $"⚠️ Committed {FilesCommitted} files but push failed: {ErrorMessage}";
            }
            else
            {
                return $"❌ Commit failed: {ErrorMessage}";
            }
        }
    }
}
