using System.Globalization;
using System.Windows.Data;

namespace Lifes.Presentation.WPF.Features.DashboardChart.Converters;

public class IndexToGridColumnConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is int index)
        {
            return index switch
            {
                0 or 9 or 10 or 11 => 0, // Left Column
                1 or 8 => 1,             // Second Column
                2 or 7 => 2,             // Third Column
                3 or 4 or 5 or 6 => 3,   // Right Column
                _ => 0
            };
        }
        return 0;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
