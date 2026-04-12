namespace ETLTools.Domain.Features.VersionIncrease.ValueObjects;

/// <summary>
/// Value object representing Git commit information.
/// </summary>
public class GitCommitInfo
{
    /// <summary>
    /// Gets the list of modified files.
    /// </summary>
    public IEnumerable<string> ModifiedFiles { get; init; } = Enumerable.Empty<string>();

    /// <summary>
    /// Gets the commit message.
    /// </summary>
    public string CommitMessage { get; init; } = string.Empty;

    /// <summary>
    /// Gets a value indicating whether to push to remote.
    /// </summary>
    public bool PushToRemote { get; init; }

    /// <summary>
    /// Gets the remote name.
    /// </summary>
    public string RemoteName { get; init; } = "origin";

    /// <summary>
    /// Gets the branch name.
    /// </summary>
    public string BranchName { get; init; } = string.Empty;
}
