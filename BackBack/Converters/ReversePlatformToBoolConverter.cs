using System;
using System.Globalization;
using System.Runtime.InteropServices;
using Avalonia.Data.Converters;

namespace BackBack.Converters
{
    public class ReversePlatformToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is OSPlatform p)
            {
                return !RuntimeInformation.IsOSPlatform(p);
            }

            throw new ArgumentException();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }
}
