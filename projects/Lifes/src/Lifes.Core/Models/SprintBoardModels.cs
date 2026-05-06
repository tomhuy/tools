using System.Collections.Generic;

namespace Lifes.Core.Models;

public class SprintTask
{
    public string Id { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string AssigneeId { get; set; } = "pre";
    public bool Done { get; set; } = false;
    public bool IsTopPriority { get; set; } = false;
}

public class Epic
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Color { get; set; } = "blue";
    public bool Archived { get; set; } = false;
    public string Status { get; set; } = "backlog"; // "progress" | "backlog"
    public List<SprintTask> Tasks { get; set; } = new();
}
