using Lifes.Infrastructure.Features.VersionIncrease.Services;
using Lifes.Infrastructure.Tests.TestUtilities;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Lifes.Infrastructure.Tests.Features.VersionIncrease.Services;

public class ProjectScannerTests : IDisposable
{
    private readonly FileSystemTestHelper _tempFiles;
    private readonly ProjectScanner _scanner;

    public ProjectScannerTests()
    {
        _tempFiles = new FileSystemTestHelper();
        var mockLogger = new Mock<ILogger<ProjectScanner>>();
        _scanner = new ProjectScanner(mockLogger.Object);
    }

    [Fact]
    public async Task ScanProjectsAsync_ShouldReturnMatchingFiles()
    {
        // Arrange
        _tempFiles.CreateTestProjectStructure();

        // Act
        var result = await _scanner.ScanProjectsAsync(_tempFiles.TestDirectory);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(2); // Test1.ETL.csproj, Test2.ETL.csproj. ShareProject is filtered out.
        result.Value.Should().Contain(x => x.EndsWith("Test1.ETL.csproj"));
        result.Value.Should().Contain(x => x.EndsWith("Test2.ETL.csproj"));
        result.Value.Should().NotContain(x => x.EndsWith("ShareProject.ETL.csproj"));
    }

    [Fact]
    public async Task ScanProjectsAsync_WithInvalidPath_ShouldReturnFailure()
    {
        // Act
        var result = await _scanner.ScanProjectsAsync("Z:\\NonExistentPath");

        // Assert
        result.IsSuccess.Should().BeFalse();
    }

    public void Dispose()
    {
        _tempFiles.Dispose();
    }
}
