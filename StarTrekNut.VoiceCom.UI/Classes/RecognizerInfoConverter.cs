using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Speech.Recognition;
using System.Windows.Data;

namespace StarTrekNut.VoiceCom.UI.Classes
{
    public class RecognizerInfoConverter : IValueConverter
    {
        #region Public Methods and Operators

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is List<RecognizerInfo> list)
                return list.Select(recog => recog.Description).ToList();
            if (value is RecognizerInfo recogInfo)
                return recogInfo.Description;

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}