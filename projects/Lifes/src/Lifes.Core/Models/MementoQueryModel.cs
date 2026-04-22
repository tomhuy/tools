using System;
using System.Collections.Generic;

namespace Lifes.Core.Models;

public class MementoQueryModel
{
    public List<int>? TagIds { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public bool? ParentOnly { get; set; }
    public string? Keyword { get; set; }
}
