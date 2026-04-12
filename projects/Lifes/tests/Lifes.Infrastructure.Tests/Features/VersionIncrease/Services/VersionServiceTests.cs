using Lifes.Core.Interfaces;
using Lifes.Infrastructure.Features.VersionIncrease.Services;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Lifes.Infrastructure.Tests.Features.VersionIncrease.Services;

public class VersionServiceTests
{
    private readonly VersionService _service;

    public VersionServiceTests()
    {
        var mockLogger = new Mock<ILogger<VersionService>>();
        _service = new VersionService(mockLogger.Object);
    }

    [Fact]
    public void ParseVersion_ValidString_ShouldReturnSuccess()
    {
        // Act
        var result = _service.ParseVersion("2026.2.3.1");

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.Year.Should().Be(2026);
    }

    [Fact]
    public void ParseVersion_InvalidString_ShouldReturnFailure()
    {
        // Act
        var result = _service.ParseVersion("invalid");

        // Assert
        result.IsSuccess.Should().BeFalse();
    }

    [Fact]
    public void IncrementVersion_ShouldReturnIncrementedValue()
    {
        // Arrange
        var version = new VersionInfo { Year = 2026, Month = 2, Day = 3, Build = 1 };
        var targetDate = new DateTime(2026, 2, 3);

        // Act
        var result = _service.IncrementVersion(version, targetDate);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value!.Build.Should().Be(2);
    }

    [Fact]
    public void FormatVersion_ShouldReturnCorrectString()
    {
        // Arrange
        var version = new VersionInfo { Year = 2026, Month = 2, Day = 3, Build = 1 };

        // Act
        var result = _service.FormatVersion(version);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be("2026.2.3.1");
    }
}
