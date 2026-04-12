using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows.Controls;
using Lifes.Presentation.WPF.Features.DashboardChart.Attributes;
using Lifes.Presentation.WPF.Features.DashboardChart.Interfaces;
using UserControl = System.Windows.Controls.UserControl;

namespace Lifes.Presentation.WPF.Features.DashboardChart.Registries;

public static class DashboardViewRegistry
{
    private static readonly Dictionary<string, Type> _viewTypes = new();

    static DashboardViewRegistry()
    {
        ScanAndRegister();
    }

    private static void ScanAndRegister()
    {
        var types = Assembly.GetExecutingAssembly().GetTypes()
            .Where(t => typeof(IDashboardBlockView).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract);

        foreach (var type in types)
        {
            var attr = type.GetCustomAttribute<DashboardBlockAttribute>();
            if (attr != null && !string.IsNullOrEmpty(attr.BlockType))
            {
                _viewTypes[attr.BlockType] = type;
            }
        }
    }

    public static UserControl? CreateView(string blockType)
    {
        if (!string.IsNullOrEmpty(blockType) && _viewTypes.TryGetValue(blockType, out var type))
        {
            return Activator.CreateInstance(type) as UserControl;
        }
        return null; // Return null so Host can fallback to Default view
    }
}
