using System;
using System.Collections.Generic;

namespace Lifes.Core.Models;

public class CalendarEventModel
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Title { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    
    // Defines the Color / Tag of the event
    public string Category { get; set; } = string.Empty;

    // Optional list of specific phases for this event
    public List<CalendarEventPhaseModel>? Phases { get; set; }
}

public class CalendarEventPhaseModel
{
    public string Title { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    
    // Optional category override for this phase
    public string? Category { get; set; }
}
