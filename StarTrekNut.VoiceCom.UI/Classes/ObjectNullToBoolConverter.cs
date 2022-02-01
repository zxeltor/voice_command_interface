using System;
using System.Globalization;
using System.Windows.Data;

namespace StarTrekNut.VoiceCom.UI.Classes
{
    public class ObjectNullToBoolConverter : IValueConverter
    {
        #region Public Methods and Operators

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value == null ? false : true;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}