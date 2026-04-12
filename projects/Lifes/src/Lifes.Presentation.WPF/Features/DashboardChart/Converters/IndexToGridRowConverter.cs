using System.Globalization;
using System.Windows.Data;

namespace Lifes.Presentation.WPF.Features.DashboardChart.Converters;

public class IndexToGridRowConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is int index)
        {
            return index switch
            {
                0 or 1 or 2 or 3 => 0, // Top Row
                4 or 11 => 1,          // Second Row
                5 or 10 => 2,          // Third Row
                6 or 7 or 8 or 9 => 3, // Bottom Row
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
