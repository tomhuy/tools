using Lifes.Domain.Features.VersionIncrease.ValueObjects;
using FluentAssertions;
using Xunit;

namespace Lifes.Domain.Tests.Features.VersionIncrease.ValueObjects;

public class VersionInfoTests
{
    [Theory]
    [InlineData("2026.2.3.1", 2026, 2, 3, 1)]
    [InlineData("2026.12.25.5", 2026, 12, 25, 5)]
    public void Parse_ValidFormat_ShouldParseCorrectly(
        string versionString, int year, int month, int day, int build)
    {
        // Act
        var result = VersionInfo.Parse(versionString);

        // Assert
        result.Should().NotBeNull();
        result!.Year.Should().Be(year);
        result.Month.Should().Be(month);
        result.Day.Should().Be(day);
        result.Build.Should().Be(build);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("Invalid")]
    [InlineData("2026.2.3")]
    [InlineData("2026.2.3.1.5")]
    [InlineData("abcd.ef.gh.ij")]
    [InlineData("2026.13.1.1")] // Invalid month
    [InlineData("2026.2.30.1")] // Invalid day for February
    public void Parse_InvalidFormat_ShouldReturnNull(string versionString)
    {
        // Act
        var result = VersionInfo.Parse(versionString);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void Increment_SameDay_ShouldIncrementBuildNumber()
    {
        // Arrange
        var version = new VersionInfo { Year = 2026, Month = 2, Day = 3, Build = 1 };
        var targetDate = new DateTime(2026, 2, 3);

        // Act
        var result = version.Increment(targetDate);

        // Assert
        result.Year.Should().Be(2026);
        result.Month.Should().Be(2);
        result.Day.Should().Be(3);
        result.Build.Should().Be(2);
    }

    [Fact]
    public void Increment_NewDay_ShouldResetBuildToOne()
    {
        // Arrange
        var version = new VersionInfo { Year = 2026, Month = 2, Day = 2, Build = 5 };
        var targetDate = new DateTime(2026, 2, 3);

        // Act
        var result = version.Increment(targetDate);

        // Assert
        result.Year.Should().Be(2026);
        result.Month.Should().Be(2);
        result.Day.Should().Be(3);
        result.Build.Should().Be(1);
    }

    [Fact]
    public void ToString_ShouldReturnCorrectFormat()
    {
        // Arrange
        var version = new VersionInfo { Year = 2026, Month = 2, Day = 3, Build = 1 };

        // Act
        var result = version.ToString();

        // Assert
        result.Should().Be("2026.2.3.1");
    }
}
