using System;
using System.Globalization;
using System.Windows.Data;
using BackBack.Models;

namespace BackBack.Converters
{
    public class TriggerTypeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is TriggerType trigger)
            {
                switch (trigger)
                {
                    case TriggerType.None:
                        return "None";
                    case TriggerType.TimedTrigger:
                        return "Timed";
                    case TriggerType.BackupItemTrigger:
                        return "BackupItem";
                }
            }

            throw new ArgumentException(value?.ToString());
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string str)
            {
                if (Enum.TryParse(str, out TriggerType trigger))
                {
                    return trigger;
                }
                else
                {
                    switch (str)
                    {
                        case "Timed":
                            return TriggerType.TimedTrigger;
                        case "BackupItem":
                            return TriggerType.BackupItemTrigger;
                        case "None":
                            return TriggerType.None;
                    }
                }
            }

            throw new ArgumentException(value?.ToString());
        }
    }

}
