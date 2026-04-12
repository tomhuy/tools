using Lifes.Infrastructure.Features.VersionIncrease.Services;
using Lifes.Infrastructure.Tests.TestUtilities;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Lifes.Infrastructure.Tests.Features.VersionIncrease.Services;

public class ProjectFileServiceTests : IDisposable
{
    private readonly FileSystemTestHelper _tempFiles;
    private readonly ProjectFileService _service;

    public ProjectFileServiceTests()
    {
        _tempFiles = new FileSystemTestHelper();
        var mockLogger = new Mock<ILogger<ProjectFileService>>();
        _service = new ProjectFileService(mockLogger.Object);
    }

    [Fact]
    public async Task ReadVersionAsync_ValidFile_ShouldReturnVersion()
    {
        // Arrange
        var filePath = _tempFiles.CreateTestCsprojFile("Test.ETL.csproj", "2026.2.3.1");

        // Act
        var result = await _service.ReadVersionAsync(filePath);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be("2026.2.3.1");
    }

    [Fact]
    public async Task UpdateVersionAsync_ValidFile_ShouldUpdateVersion()
    {
        // Arrange
        var filePath = _tempFiles.CreateTestCsprojFile("Test.ETL.csproj", "2026.2.3.1");
        var newVersion = "2026.2.3.2";

        // Act
        var result = await _service.UpdateVersionAsync(filePath, newVersion);

        // Assert
        result.IsSuccess.Should().BeTrue();

        // Verify content
        var verifyResult = await _service.ReadVersionAsync(filePath);
        verifyResult.Value.Should().Be(newVersion);
    }

    public void Dispose()
    {
        _tempFiles.Dispose();
    }
}
