namespace Lifes.Domain.Features.VersionIncrease.ValueObjects;

/// <summary>
/// Value object representing version information in yyyy.M.d.{build} format.
/// </summary>
public class VersionInfo
{
    public int Year { get; init; }
    public int Month { get; init; }
    public int Day { get; init; }
    public int Build { get; init; }

    /// <summary>
    /// Gets the date portion of the version.
    /// </summary>
    public DateTime GetDate() => new DateTime(Year, Month, Day);

    /// <summary>
    /// Converts the version to string format: yyyy.M.d.{build}
    /// </summary>
    public override string ToString() => $"{Year}.{Month}.{Day}.{Build}";

    /// <summary>
    /// Parses a version string into a VersionInfo object.
    /// </summary>
    /// <param name="versionString">Version string in format yyyy.M.d.{build}.</param>
    /// <returns>Parsed VersionInfo or null if invalid.</returns>
    public static VersionInfo? Parse(string versionString)
    {
        if (string.IsNullOrWhiteSpace(versionString))
            return null;

        var parts = versionString.Split('.');
        if (parts.Length != 4)
            return null;

        if (!int.TryParse(parts[0], out int year) ||
            !int.TryParse(parts[1], out int month) ||
            !int.TryParse(parts[2], out int day) ||
            !int.TryParse(parts[3], out int build))
        {
            return null;
        }

        // Validate date components
        if (year < 1 || month < 1 || month > 12 || day < 1 || day > 31 || build < 1)
            return null;

        try
        {
            // Verify it's a valid date
            _ = new DateTime(year, month, day);
        }
        catch
        {
            return null;
        }

        return new VersionInfo
        {
            Year = year,
            Month = month,
            Day = day,
            Build = build
        };
    }

    /// <summary>
    /// Increments the version based on the target date.
    /// Business Logic:
    /// - If current version date == target date: increment build number
    /// - If current version date != target date: reset build to 1
    /// </summary>
    /// <param name="targetDate">The target date to increment to.</param>
    /// <returns>New incremented VersionInfo.</returns>
    public VersionInfo Increment(DateTime targetDate)
    {
        var currentDate = GetDate().Date;
        var targetDateOnly = targetDate.Date;

        if (currentDate == targetDateOnly)
        {
            // Same day: increment build number
            return new VersionInfo
            {
                Year = targetDate.Year,
                Month = targetDate.Month,
                Day = targetDate.Day,
                Build = Build + 1
            };
        }
        else
        {
            // Different day: reset build to 1
            return new VersionInfo
            {
                Year = targetDate.Year,
                Month = targetDate.Month,
                Day = targetDate.Day,
                Build = 1
            };
        }
    }

    /// <summary>
    /// Checks if the version date is the same as the specified date.
    /// </summary>
    public bool IsSameDate(DateTime date)
    {
        return GetDate().Date == date.Date;
    }
}
