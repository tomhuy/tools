namespace Lifes.Application.Features.VersionIncrease.DTOs;

/// <summary>
/// Data transfer object for commit changes request.
/// </summary>
public class CommitChangesDto
{
    /// <summary>
    /// Gets or sets the list of modified files to commit.
    /// </summary>
    public IEnumerable<string> ModifiedFiles { get; set; } = Enumerable.Empty<string>();

    /// <summary>
    /// Gets or sets the commit message.
    /// </summary>
    public string CommitMessage { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets a value indicating whether to push to remote after commit.
    /// </summary>
    public bool PushToRemote { get; set; } = true;

    /// <summary>
    /// Gets or sets the remote name (default: "origin").
    /// </summary>
    public string RemoteName { get; set; } = "origin";

    /// <summary>
    /// Gets or sets the branch name. If null, uses current branch.
    /// </summary>
    public string? BranchName { get; set; }

    /// <summary>
    /// Gets or sets the repository path.
    /// </summary>
    public string RepositoryPath { get; set; } = string.Empty;
}
