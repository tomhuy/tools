namespace Lifes.Domain.Features.DashboardChart.Entities;

public class DashboardBlock
{
    public int Index { get; set; }
    public string BlockType { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public object? Data { get; set; }
}
