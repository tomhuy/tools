using System;

namespace Lifes.Presentation.WPF.Features.DashboardChart.Attributes;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public class DashboardBlockAttribute : Attribute
{
    public string BlockType { get; }

    public DashboardBlockAttribute(string blockType)
    {
        BlockType = blockType;
    }
}
