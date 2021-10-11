using System;
using System.Globalization;
using System.Windows.Data;

namespace StarTrekNut.VoiceCom.UI.Classes
{
    public class IsEnabledConverter : IValueConverter
    {
        #region Public Methods and Operators

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value != null && !(bool)value ? "Disable the speech recognition engine" : "You must save settings before enabling speech recognition";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}