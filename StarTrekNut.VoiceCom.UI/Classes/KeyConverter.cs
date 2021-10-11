using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Input;

namespace StarTrekNut.VoiceCom.UI.Classes
{
    public class KeyConverter : IValueConverter
    {
        #region Public Methods and Operators

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Key keyTmpKey)
                return keyTmpKey.ToString();

            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}