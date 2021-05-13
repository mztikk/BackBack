using System;
using System.Globalization;
using Avalonia;
using Avalonia.Data.Converters;

namespace BackBack.Converters
{
    public class BoolToMarginSelection : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool b && parameter is string s)
            {
                string[]? split = s.Split(',');
                if (split.Length == 2 && int.TryParse(split[0], out int x) && int.TryParse(split[1], out int y))
                {
                    return b ? new Thickness(x) : new Thickness(y);
                }
            }

            throw new ArgumentException();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }
}
