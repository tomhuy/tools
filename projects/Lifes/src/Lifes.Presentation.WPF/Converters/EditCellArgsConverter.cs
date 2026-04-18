using System;
using System.Globalization;
using System.Windows.Data;
using Lifes.Presentation.WPF.Features.AnnualCalendar;

namespace Lifes.Presentation.WPF.Converters;

public class EditCellArgsConverter : IMultiValueConverter
{
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        if (values.Length >= 3 && 
            values[0] is MonthlyEventRowViewModel row && 
            values[1] is int month && 
            values[2] is int day)
        {
            var args = new EditCellArgs
            {
                Row = row,
                Month = month,
                Day = day
            };

            if (values.Length >= 4 && values[3] is int mementoId && mementoId != 0)
            {
                args.ExistingMementoId = mementoId;
            }

            return args;
        }
        return null!;
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
