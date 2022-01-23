using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace StarTrekNut.VoiceCom.Lib.Helpers
{
    public static class KeyTranslationHelper
    {
        public static string GetVisualKeyString(List<Key> keys)
        {
            var resultString = new StringBuilder();

            var keyCnt = 0;
            keys.ForEach(key => 
            {
                resultString.Append(key.ToString());
                
                if(keyCnt++<keys.Count-1)
                {
                    resultString.Append("+");
                }

            });

            return resultString.ToString();
        }
    }
}
