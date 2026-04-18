using System;

namespace Lifes.Core.Models;

public class MementoModel
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    
    // Hierarchical link: null means Topic Note, non-null means Supplemental Concept Note
    public int? ParentId { get; set; }
    
    // Sort order
    public int Order { get; set; }
    
    public DateTime CreatedDate { get; set; } = DateTime.Now;
    
    // Visual category/color
    public string Color { get; set; } = string.Empty;
    
    public string Description { get; set; } = string.Empty;
    
    public List<int> TagIds { get; set; } = new();
}
