using Lifes.Domain.Features.VersionIncrease.Entities;
using Lifes.Domain.Features.VersionIncrease.ValueObjects;

namespace Lifes.Infrastructure.Tests.TestUtilities;

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
