using Lifes.Domain.Features.VersionIncrease.Entities;
using FluentAssertions;
using Xunit;

namespace Lifes.Domain.Tests.Features.VersionIncrease.Entities;

public class ProjectFileTests
{
    [Theory]
    [InlineData("Test.ETL.csproj", true)]
    [InlineData("MyProject.ETL.csproj", true)]
    [InlineData("ETL.csproj", true)]
    [InlineData("ShareProject.ETL.csproj", false)] // Starts with Share
    [InlineData("Test.csproj", false)] // Doesn't end with ETL
    [InlineData("Test.ETL.txt", false)] // Wrong extension
    [InlineData("SHAREService.ETL.csproj", false)] // Case-insensitive Share
    [InlineData("TestETL.csproj", true)] // Ends with ETL
    public void MatchesFilter_ShouldFollowBusinessRules(string fileName, bool expectedResult)
    {
        // Arrange
        var projectFile = new ProjectFile { FileName = fileName };

        // Act
        var result = projectFile.MatchesFilter();

        // Assert
        result.Should().Be(expectedResult);
    }
}
