using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

using StarTrekNut.VoiceCom.Lib.Model;

namespace StarTrekNut.VoiceCom.UI.Classes
{
    public class EntryTypeColorConverter : IValueConverter
    {
        #region Public Methods and Operators

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is LogEntryType)
            {
                var entryType = (LogEntryType)value;

                switch (entryType)
                {
                    case LogEntryType.Info:
                        return new SolidColorBrush(Colors.Gray);
                    case LogEntryType.Warning:
                        return new SolidColorBrush(Colors.Orange);
                    case LogEntryType.Error:
                        return new SolidColorBrush(Colors.Red);
                    case LogEntryType.Debug:
                        return new SolidColorBrush(Colors.Black);
                    case LogEntryType.Unknown:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            return new SolidColorBrush(Colors.Black);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}