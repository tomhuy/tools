namespace Lifes.Domain.Features.DashboardChart.Entities;

public class AstrologyBlockData
{
    public string HeaderText { get; set; } = string.Empty;
    public string MinorText { get; set; } = string.Empty;
    public string[] Elements { get; set; } = Array.Empty<string>();
}
