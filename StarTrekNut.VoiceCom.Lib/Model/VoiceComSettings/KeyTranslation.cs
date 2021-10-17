using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Text.RegularExpressions;

namespace StarTrekNut.VoiceCom.Lib.Model.VoiceComSettings
{
    public class KeyTranslation : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private bool _enabled;
        private Key _key;
        private string _translation;
        private static readonly Regex _regexAltKey = new Regex(@"%\((.+)\)", System.Text.RegularExpressions.RegexOptions.Compiled);
        private static readonly Regex _regexShiftKey = new Regex(@"\+\((.+)\)", System.Text.RegularExpressions.RegexOptions.Compiled);
        private static readonly Regex _regexControlKey = new Regex(@"\^\((.+)\)", System.Text.RegularExpressions.RegexOptions.Compiled);

        public KeyTranslation() { }
        public KeyTranslation(Key key, string translation)
        {
            this.Key = key;
            this.Translation = translation;
            this.Enabled = true;
        }
        public bool Enabled
        {
            get => this._enabled;
            set
            {
                this._enabled = value;
                this.NotifyPropertyChange("Enabled");
            }
        }

        public Key Key
        {
            get => this._key;
            set
            {
                this._key = value;
                this.NotifyPropertyChange("Key");
                this.NotifyPropertyChange("KeyString");
            }
        }

        [System.Xml.Serialization.XmlIgnore]
        public string KeyString => Key.ToString();

        public string Translation
        {
            get => this._translation;
            set
            {
                this._translation = value;
                this.NotifyPropertyChange("Translation");
            }
        }

        private void NotifyPropertyChange(string propertyName)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public static List<KeyTranslation> DEFAULT_KEY_TRANSLATIONS = new List<KeyTranslation>()
        {
            new KeyTranslation(Key.Back, "{BKSP}"),
            new KeyTranslation(Key.CapsLock, "{CAPSLOCK}"),
            new KeyTranslation(Key.Delete, "{DEL}"),
            new KeyTranslation(Key.Down, "{DOWN}"),
            new KeyTranslation(Key.End, "{END}"),
            new KeyTranslation(Key.Enter, "{ENTER}"),
            new KeyTranslation(Key.Escape, "{ESC}"),
            new KeyTranslation(Key.Help, "{HELP}"),
            new KeyTranslation(Key.Home, "{HOME}"),
            new KeyTranslation(Key.Insert, "{INS}"),
            new KeyTranslation(Key.Left, "{LEFT}"),
            new KeyTranslation(Key.NumLock, "{NUMLOCK}"),
            new KeyTranslation(Key.PageDown, "{PGDN}"),
            new KeyTranslation(Key.PageUp, "{PGUP}"),
            new KeyTranslation(Key.PrintScreen, "{PRTSC}"),
            new KeyTranslation(Key.Right, "{RIGHT}"),
            new KeyTranslation(Key.Scroll, "{SCROLLLOCK}"),
            new KeyTranslation(Key.Tab, "{TAB}"),
            new KeyTranslation(Key.Up, "{UP}"),
            new KeyTranslation(Key.F1, "{F1}"),
            new KeyTranslation(Key.F2, "{F2}"),
            new KeyTranslation(Key.F3, "{F3}"),
            new KeyTranslation(Key.F4, "{F4}"),
            new KeyTranslation(Key.F5, "{F5}"),
            new KeyTranslation(Key.F6, "{F6}"),
            new KeyTranslation(Key.F7, "{F7}"),
            new KeyTranslation(Key.F8, "{F8}"),
            new KeyTranslation(Key.F9, "{F9}"),
            new KeyTranslation(Key.F10, "{F10}"),
            new KeyTranslation(Key.F11, "{F11}"),
            new KeyTranslation(Key.F12, "{F12}"),
            new KeyTranslation(Key.F13, "{F13}"),
            new KeyTranslation(Key.F14, "{F14}"),
            new KeyTranslation(Key.F15, "{F15}"),
            new KeyTranslation(Key.F16, "{F16}"),
            new KeyTranslation(Key.Add, "{ADD}"),
            new KeyTranslation(Key.D0, "0"),
            new KeyTranslation(Key.D1, "1"),
            new KeyTranslation(Key.D2, "2"),
            new KeyTranslation(Key.D3, "3"),
            new KeyTranslation(Key.D4, "4"),
            new KeyTranslation(Key.D5, "5"),
            new KeyTranslation(Key.D6, "6"),
            new KeyTranslation(Key.D7, "7"),
            new KeyTranslation(Key.D8, "8"),
            new KeyTranslation(Key.D9, "9"),
            new KeyTranslation(Key.Subtract, "{SUBTRACT}"),
            new KeyTranslation(Key.Multiply, "{MULTIPLY}"),
            new KeyTranslation(Key.Divide, "{DIVIDE}")
            // The plus sign (+), caret (^), percent sign (%), tilde (~), and parentheses () have special meanings to
        };

        public static string GetSendKeysString(Key key, ObservableCollection<KeyTranslation> keyTranslations)
        {            
            var resultString = string.Empty;

            var keyTranslation = keyTranslations.FirstOrDefault(keyTran => keyTran.Key == key);

            if (keyTranslation != null)
            {
                resultString = keyTranslation.Translation;
            }
            else
            {
                resultString = key.ToString();
            }

            // SHIFT +
            // CTRL ^
            // ALT %

            if (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift))
            {
                resultString = $"+({resultString})";
            }
            if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
            {
                resultString = $"^({resultString})";
            }
            if (Keyboard.IsKeyDown(Key.LeftAlt) || Keyboard.IsKeyDown(Key.RightAlt))
            {
                resultString = $"%({resultString})";
            }

            return resultString;
        }
        
        public static string GetVisualKeyString(Key key)
        {
            var resultString = new StringBuilder();

            if (Keyboard.IsKeyDown(Key.LeftShift)) resultString.Append(Key.LeftShift.ToString()).Append("+");
            if (Keyboard.IsKeyDown(Key.RightShift)) resultString.Append(Key.RightShift.ToString()).Append("+");
            if (Keyboard.IsKeyDown(Key.LeftCtrl)) resultString.Append(Key.LeftCtrl.ToString()).Append("+");
            if (Keyboard.IsKeyDown(Key.RightCtrl)) resultString.Append(Key.RightCtrl.ToString()).Append("+");
            if (Keyboard.IsKeyDown(Key.LeftAlt)) resultString.Append(Key.LeftAlt.ToString()).Append("+");
            if (Keyboard.IsKeyDown(Key.RightAlt)) resultString.Append(Key.RightAlt.ToString()).Append("+");

            // SHIFT +
            // CTRL ^
            // ALT %

            return resultString.Append(key.ToString()).ToString();
        }

        private static string GetTranslatedKey(string sendKeysString, List<KeyTranslation> keyTranslations)
        {
            var keyTranslation = keyTranslations.FirstOrDefault(tran => tran.Translation.Equals(sendKeysString, StringComparison.CurrentCultureIgnoreCase));
            if (keyTranslation == null)
                return sendKeysString;
            else
                return keyTranslation.Key.ToString();
        }

        public static string GetVisualStringFromSendKeysString(string sendKeysString, List<KeyTranslation> keyTranslations)
        {
            if (string.IsNullOrWhiteSpace(sendKeysString)) return sendKeysString;

            var hasCtrl = _regexControlKey.IsMatch(sendKeysString);
            var hasShift = _regexShiftKey.IsMatch(sendKeysString);
            var hasAlt = _regexAltKey.IsMatch(sendKeysString);

            if(!hasAlt && !hasCtrl && !hasShift)
            {
                return GetTranslatedKey(sendKeysString, keyTranslations);
            }

            var matchesAlt = _regexAltKey.Matches(sendKeysString);
            foreach (Match match in matchesAlt)
            {
                var matchString = match.Groups[0].Value;
                var strippedString = match.Groups[1].Value;
                sendKeysString = sendKeysString.Replace(matchString, $"{Key.LeftAlt}+{GetTranslatedKey(strippedString, keyTranslations)}");
            }

            var matchesCtrl = _regexControlKey.Matches(sendKeysString);
            foreach(Match match in matchesCtrl)
            {
                var matchString = match.Groups[0].Value;
                var strippedString = match.Groups[1].Value;
                sendKeysString = sendKeysString.Replace(matchString, $"{Key.LeftCtrl}+{GetTranslatedKey(strippedString, keyTranslations)}");
            }

            var matchesShift = _regexShiftKey.Matches(sendKeysString);
            foreach (Match match in matchesShift)
            {
                var matchString = match.Groups[0].Value;
                var strippedString = match.Groups[1].Value;
                sendKeysString = sendKeysString.Replace(matchString, $"{Key.LeftShift}+{GetTranslatedKey(strippedString, keyTranslations)}");
            }

            return sendKeysString;
        }

        public static KeyTranslation Clone(KeyTranslation keyTranslation)
        {
            return new KeyTranslation(keyTranslation.Key, keyTranslation.Translation);
        }
    }
}
