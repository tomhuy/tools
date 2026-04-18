using System;
using System.Globalization;
using System.Windows.Data;
using Lifes.Presentation.WPF.Features.AnnualCalendar;

namespace Lifes.Presentation.WPF.Converters;

public class EditCellArgsConverter : IMultiValueConverter
{
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        if (values.Length >= 3 && values[0] is MonthlyEventRowViewModel row)
        {
            int month = 1;
            int day = 1;

            if (values[1] is int m) month = m;
            else if (values[1] is string sm && int.TryParse(sm, out int pm)) month = pm;

            if (values[2] is int d) day = d;
            else if (values[2] is string sd && int.TryParse(sd, out int pd)) day = pd;

            var args = new EditCellArgs
            {
                Row = row,
                Month = month,
                Day = day
            };

            if (values.Length >= 4)
            {
                int mId = 0;
                if (values[3] is int mi) mId = mi;
                else if (values[3] is string si && int.TryParse(si, out int pi)) mId = pi;

                if (mId != 0)
                {
                    args.ExistingMementoId = mId;
                }
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
