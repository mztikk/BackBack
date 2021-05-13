using System;
using System.Globalization;
using Avalonia;
using Avalonia.Data.Converters;

namespace BackBack.Converters
{
    public class ReverseBoolToThicknessConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool b && (parameter is int i || (parameter is string s && int.TryParse(s, out i))))
            {
                return b ? new Thickness(0) : new Thickness(i);
            }

            throw new ArgumentException();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }
}
