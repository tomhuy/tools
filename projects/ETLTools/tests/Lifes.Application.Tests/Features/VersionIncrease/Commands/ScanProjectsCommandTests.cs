using Lifes.Application.Features.VersionIncrease.Commands;
using Lifes.Application.Tests.TestUtilities;
using Lifes.Core.Interfaces;
using Lifes.Core.Models;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Lifes.Application.Tests.Features.VersionIncrease.Commands;

public class ScanProjectsCommandTests
{
    private readonly Mock<IProjectScanner> _mockScanner;
    private readonly Mock<IProjectFileService> _mockFileService;
    private readonly Mock<ILogger<ScanProjectsCommand>> _mockLogger;
    private readonly ScanProjectsCommand _command;

    public ScanProjectsCommandTests()
    {
        _mockScanner = AppMockFactory.CreateProjectScannerMock();
        _mockFileService = AppMockFactory.CreateProjectFileServiceMock();
        _mockLogger = new Mock<ILogger<ScanProjectsCommand>>();
        _command = new ScanProjectsCommand(
            _mockScanner.Object,
            _mockFileService.Object,
            _mockLogger.Object);
    }

    [Fact]
    public async Task ExecuteAsync_WhenProjectsFound_ShouldReturnDtos()
    {
        // Arrange
        var basePath = "C:\\Root";
        var projectFiles = new[] { "C:\\Root\\Project1.ETL.csproj", "C:\\Root\\Sub\\Project2.ETL.csproj" };
        
        _mockScanner.Setup(x => x.ScanProjectsAsync(basePath, It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(Result<IEnumerable<string>>.Success(projectFiles));
        
        _mockFileService.Setup(x => x.ReadVersionAsync(It.IsAny<string>()))
            .ReturnsAsync(Result<string>.Success("2026.2.3.1"));

        // Act
        var result = await _command.ExecuteAsync(basePath);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(2);
        result.Value!.First().FileName.Should().Be("Project1.ETL.csproj");
        result.Value!.First().CurrentVersion.Should().Be("2026.2.3.1");
    }

    [Fact]
    public async Task ExecuteAsync_WhenScannerFails_ShouldReturnFailure()
    {
        // Arrange
        var basePath = "C:\\Root";
        _mockScanner.Setup(x => x.ScanProjectsAsync(basePath, It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(Result<IEnumerable<string>>.Failure("Scan error"));

        // Act
        var result = await _command.ExecuteAsync(basePath);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Scan error");
    }
}
