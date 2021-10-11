using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StarTrekNut.VoiceCom.Lib.Model.VoiceComSettings;

using System.Windows.Input;

namespace StarTrekNut.VoiceCom.Lib.Classes
{
    public static class KeyLookup
    {
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

        //public static List<KeyTranslation> DEFAULT_KEY_TRANSLATIONS { get => DEFAULT_KEY_TRANSLATIONS; set => DEFAULT_KEY_TRANSLATIONS = value; }

        public static string GetSendKeysString(bool hasShiftMod, bool hasCtrlMod, bool hasAltMod, Key key, List<KeyTranslation> keyTranslations)
        {
            var resultString = string.Empty;

            var keyTranslation = keyTranslations.FirstOrDefault(keyTran => keyTran.Key == key);

            if(keyTranslation != null)
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

            if (hasShiftMod)
            {
                resultString = $"+({resultString})";
            }
            if (hasCtrlMod)
            {
                resultString = $"^({resultString})";
            }
            if (hasAltMod)
            {
                resultString = $"%({resultString})";
            }

            return resultString;
        }
    }
}
