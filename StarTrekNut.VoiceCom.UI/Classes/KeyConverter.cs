using System;
using System.Collections.Generic;
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
            if (value is List<Key> keyList)
            {
                return Lib.Helpers.KeyTranslationHelper.GetVisualKeyString(keyList);
            }
            else if (value is Key key)
                return key.ToString();
            
            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}