using System;
using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia.Media;

namespace BackBack.Converters
{
    public class BoolToBrushTransparencyConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool b)
            {
                return b ? parameter : Brushes.Transparent;
            }

            throw new ArgumentException();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }
}
