namespace Lifes.Application.Features.VersionIncrease.DTOs;

/// <summary>
/// Data transfer object for individual project update result.
/// </summary>
public class ProjectUpdateDto
{
    public string ProjectName { get; set; } = string.Empty;
    public string OldVersion { get; set; } = string.Empty;
    public string NewVersion { get; set; } = string.Empty;
    public bool Success { get; set; }
    public string ErrorMessage { get; set; } = string.Empty;
}
