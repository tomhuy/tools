using ETLTools.Domain.Features.VersionIncrease.Entities;
using ETLTools.Domain.Features.VersionIncrease.Enums;
using ETLTools.Domain.Features.VersionIncrease.ValueObjects;

namespace ETLTools.Domain.Tests.TestUtilities;

public static class DomainTestFixtures
{
    public static ProjectFile CreateTestProjectFile(
        string fileName = "Test.ETL.csproj",
        string version = "2026.2.3.1")
    {
        return new ProjectFile
        {
            FileName = fileName,
            FullPath = $"C:\\Projects\\{fileName}",
            RelativePath = $"Projects\\{fileName}",
            CurrentVersion = VersionInfo.Parse(version),
            Status = ProjectStatus.Ready
        };
    }

    public static VersionInfo CreateTestVersionInfo(
        int year = 2026,
        int month = 2,
        int day = 3,
        int build = 1)
    {
        return new VersionInfo
        {
            Year = year,
            Month = month,
            Day = day,
            Build = build
        };
    }

    public static List<ProjectFile> CreateTestProjectList(int count = 3)
    {
        var projects = new List<ProjectFile>();
        for (int i = 1; i <= count; i++)
        {
            projects.Add(CreateTestProjectFile($"Test{i}.ETL.csproj", $"2026.2.3.{i}"));
        }
        return projects;
    }
}
