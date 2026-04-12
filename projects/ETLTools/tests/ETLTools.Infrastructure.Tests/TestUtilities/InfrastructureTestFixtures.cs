using ETLTools.Domain.Features.VersionIncrease.Entities;
using ETLTools.Domain.Features.VersionIncrease.ValueObjects;

namespace ETLTools.Infrastructure.Tests.TestUtilities;

public static class InfrastructureTestFixtures
{
    public static ProjectFile CreateTestProjectFile(string fileName, string version)
    {
        return new ProjectFile
        {
            FileName = fileName,
            FullPath = fileName,
            CurrentVersion = VersionInfo.Parse(version)
        };
    }
}
