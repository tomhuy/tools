namespace Lifes.Domain.Features.VersionIncrease.Enums;

/// <summary>
/// Represents the status of a project file during processing.
/// </summary>
public enum ProjectStatus
{
    /// <summary>
    /// Project is ready to be processed.
    /// </summary>
    Ready,

    /// <summary>
    /// Project is currently being processed.
    /// </summary>
    Processing,

    /// <summary>
    /// Project has been successfully updated.
    /// </summary>
    Updated,

    /// <summary>
    /// Project processing failed.
    /// </summary>
    Failed,

    /// <summary>
    /// Project was skipped.
    /// </summary>
    Skipped
}
