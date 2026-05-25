using System;
using System.Collections.Generic;

namespace Lifes.Core.Models;

public class MoodMetadataDefinition
{
    public string Key { get; set; } = string.Empty; // Unique key (slugified, e.g., luong_nuoc_uong)
    public string LabelDisplay { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string InputType { get; set; } = "text"; // text, number, select, checkbox, radio, textarea, date, time, datetime, duration
    public List<string> Options { get; set; } = new(); // for select, checkbox, radio
    public bool Enabled { get; set; } = true;
    public int Order { get; set; }
}
