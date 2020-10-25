using System;
using System.Globalization;
using System.Windows.Data;

namespace BackBack.Converters
{
    public class StringLengthBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string str)
            {
                return str.Length > 0 ? true : false;
            }
            else if (value is null)
            {
                return false;
            }

            throw new ArgumentException();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }

}
