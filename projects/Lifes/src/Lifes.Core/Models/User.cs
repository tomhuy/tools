using System;

namespace Lifes.Core.Models;

public class User
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Name { get; set; } = string.Empty;
    public string Initials { get; set; } = string.Empty;
    public string Color { get; set; } = "blue";
}
