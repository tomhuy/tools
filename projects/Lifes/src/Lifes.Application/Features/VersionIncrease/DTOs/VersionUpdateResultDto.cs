namespace Lifes.Application.Features.VersionIncrease.DTOs;

/// <summary>
/// Data transfer object for version update operation result.
/// </summary>
public class VersionUpdateResultDto
{
    public int TotalProjects { get; set; }
    public int SuccessCount { get; set; }
    public int FailedCount { get; set; }
    public List<ProjectUpdateDto> Updates { get; set; } = new();

    public bool IsSuccess => FailedCount == 0 && TotalProjects > 0;
    public string Summary => $"Updated {SuccessCount} of {TotalProjects} projects successfully";
}
