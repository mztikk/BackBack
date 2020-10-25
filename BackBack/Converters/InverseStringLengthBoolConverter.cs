using System;
using System.Globalization;
using System.Windows.Data;

namespace BackBack.Converters
{
    public class InverseStringLengthBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string str)
            {
                return str.Length > 0 ? false : true;
            }
            else if (value is null)
            {
                return true;
            }

            throw new ArgumentException();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }

}
