namespace Lifes.Application.Features.VersionIncrease.DTOs;

/// <summary>
/// Data transfer object for project file information.
/// </summary>
public class ProjectFileDto
{
    public string FileName { get; set; } = string.Empty;
    public string FullPath { get; set; } = string.Empty;
    public string RelativePath { get; set; } = string.Empty;
    public string CurrentVersion { get; set; } = string.Empty;
}
