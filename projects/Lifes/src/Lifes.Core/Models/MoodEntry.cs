using System;
using System.Collections.Generic;

namespace Lifes.Core.Models;

public class MoodEntry
{
    public string Id { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public string MoodId { get; set; } = string.Empty;
    public List<string> Tags { get; set; } = new();
    public string? Note { get; set; }
    public string? Reason { get; set; }
}
