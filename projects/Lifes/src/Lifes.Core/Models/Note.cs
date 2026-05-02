using System;
using System.Collections.Generic;

namespace Lifes.Core.Models;

public class Note
{
    public string Id { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public DateTime Created { get; set; } = DateTime.Now;
    public DateTime Modified { get; set; } = DateTime.Now;
    public bool IsStarred { get; set; }
    public List<string> Tags { get; set; } = new();
    public string Section { get; set; } = "Inbox";
}
