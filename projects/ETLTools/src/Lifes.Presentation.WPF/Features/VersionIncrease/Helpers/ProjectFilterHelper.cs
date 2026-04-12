using Lifes.Presentation.WPF.Features.VersionIncrease.Models;

namespace Lifes.Presentation.WPF.Features.VersionIncrease.Helpers;

/// <summary>
/// Helper class for filtering project files based on search text.
/// </summary>
public static class ProjectFilterHelper
{
    /// <summary>
    /// Filters projects based on search text.
    /// Searches in: FileName, RelativePath, CurrentVersion.
    /// </summary>
    /// <param name="projects">The collection of projects to filter.</param>
    /// <param name="searchText">The search text (case-insensitive).</param>
    /// <returns>Filtered collection of projects.</returns>
    public static IEnumerable<ProjectFileViewModel> Filter(
        IEnumerable<ProjectFileViewModel> projects,
        string searchText)
    {
        if (projects == null)
        {
            return Enumerable.Empty<ProjectFileViewModel>();
        }

        // If search text is empty or whitespace, return all projects
        if (string.IsNullOrWhiteSpace(searchText))
        {
            return projects;
        }

        var lowerSearch = searchText.ToLower();

        return projects.Where(p =>
            (p.FileName?.ToLower().Contains(lowerSearch) ?? false) ||
            (p.RelativePath?.ToLower().Contains(lowerSearch) ?? false) ||
            (p.CurrentVersion?.ToLower().Contains(lowerSearch) ?? false)
        );
    }

    /// <summary>
    /// Gets filter status text.
    /// </summary>
    /// <param name="filteredCount">Number of filtered projects.</param>
    /// <param name="totalCount">Total number of projects.</param>
    /// <param name="isFiltered">Whether filter is active.</param>
    /// <returns>Status text or empty string.</returns>
    public static string GetFilterStatusText(int filteredCount, int totalCount, bool isFiltered)
    {
        if (!isFiltered || filteredCount == totalCount)
        {
            return string.Empty;
        }

        return $"Showing {filteredCount} of {totalCount} projects";
    }
}
