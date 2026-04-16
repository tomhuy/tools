namespace Lifes.Core.Models;

using System;

public class DocumentModel
{
    public string Id { get; set; } = string.Empty;
    public string? ParentId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string Status { get; set; } = string.Empty;

    public bool IsTask => !string.IsNullOrEmpty(ParentId);
}
