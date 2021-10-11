using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Speech.Synthesis;
using System.Windows.Data;

namespace StarTrekNut.VoiceCom.UI.Classes
{
    public class TtsVoiceConverter : IValueConverter
    {
        #region Public Methods and Operators

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is List<InstalledVoice> list)
                return list.Select(voice => voice.VoiceInfo.Name).ToList();
            if (value is InstalledVoice installedVoice)
                return installedVoice.VoiceInfo.Name;
            if (value is string)
                return value;

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}