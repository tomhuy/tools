using ETLTools.Application.Features.VersionIncrease.Commands;
using ETLTools.Application.Features.VersionIncrease.DTOs;
using ETLTools.Application.Tests.TestUtilities;
using ETLTools.Core.Interfaces;
using ETLTools.Core.Models;
using CoreVersionInfo = ETLTools.Core.Interfaces.VersionInfo;
using DomainVersionInfo = ETLTools.Domain.Features.VersionIncrease.ValueObjects.VersionInfo;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace ETLTools.Application.Tests.Features.VersionIncrease.Commands;

public class UpdateVersionsCommandTests
{
    private readonly Mock<IProjectFileService> _mockFileService;
    private readonly Mock<IVersionService> _mockVersionService;
    private readonly Mock<ILogger<UpdateVersionsCommand>> _mockLogger;
    private readonly UpdateVersionsCommand _command;

    public UpdateVersionsCommandTests()
    {
        _mockFileService = AppMockFactory.CreateProjectFileServiceMock();
        _mockVersionService = AppMockFactory.CreateVersionServiceMock();
        _mockLogger = new Mock<ILogger<UpdateVersionsCommand>>();
        _command = new UpdateVersionsCommand(
            _mockFileService.Object,
            _mockVersionService.Object,
            _mockLogger.Object);
    }

    [Fact]
    public async Task ExecuteAsync_WhenSuccessful_ShouldReturnUpdateSummary()
    {
        // Arrange
        var projects = new[]
        {
            new ProjectFileDto { FileName = "P1.ETL.csproj", FullPath = "C:\\P1.csproj", CurrentVersion = "2026.2.3.1" }
        };
        var targetDate = new DateTime(2026, 2, 3);
        var versionInfo = new CoreVersionInfo { Year = 2026, Month = 2, Day = 3, Build = 1 };
        var incrementedInfo = new CoreVersionInfo { Year = 2026, Month = 2, Day = 3, Build = 2 };

        _mockVersionService.Setup(x => x.ParseVersion("2026.2.3.1"))
            .Returns(Result<CoreVersionInfo>.Success(versionInfo));
        _mockVersionService.Setup(x => x.IncrementVersion(versionInfo, targetDate))
            .Returns(Result<CoreVersionInfo>.Success(incrementedInfo));
        _mockVersionService.Setup(x => x.FormatVersion(incrementedInfo))
            .Returns(Result<string>.Success("2026.2.3.2"));
        
        _mockFileService.Setup(x => x.UpdateVersionAsync("C:\\P1.csproj", "2026.2.3.2"))
            .ReturnsAsync(Result.Success());

        // Act
        var result = await _command.ExecuteAsync(projects, targetDate);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value!.TotalProjects.Should().Be(1);
        result.Value.SuccessCount.Should().Be(1);
        result.Value.Updates.First().NewVersion.Should().Be("2026.2.3.2");
    }

    [Fact]
    public async Task ExecuteAsync_WhenUpdateFails_ShouldRecordFailure()
    {
        // Arrange
        var projects = new[]
        {
            new ProjectFileDto { FileName = "P1.ETL.csproj", FullPath = "C:\\P1.csproj", CurrentVersion = "2026.2.3.1" }
        };
        
        _mockVersionService.Setup(x => x.ParseVersion(It.IsAny<string>()))
            .Returns(Result<CoreVersionInfo>.Failure("Parse error"));

        // Act
        var result = await _command.ExecuteAsync(projects);

        // Assert
        result.IsSuccess.Should().BeTrue(); // The command itself succeeds in processing the list
        result.Value!.FailedCount.Should().Be(1);
        result.Value.Updates.First().Success.Should().BeFalse();
        result.Value.Updates.First().ErrorMessage.Should().Be("Parse error");
    }
}
