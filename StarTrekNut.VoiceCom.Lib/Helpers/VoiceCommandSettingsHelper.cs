using StarTrekNut.VoiceCom.Lib.Model.VoiceComSettings;

namespace StarTrekNut.VoiceCom.Lib.Helpers
{
    public static class VoiceCommandSettingsHelper
    {
        public static VoiceCommandSettings ReadFile(string filename)
        {
            using (var sr = new System.IO.StreamReader(filename))
            {
                var serlizer = new System.Xml.Serialization.XmlSerializer(typeof(VoiceCommandSettings));
                return serlizer.Deserialize(sr) as VoiceCommandSettings;
            }
        }

        public static void WriteFile(string filename, VoiceCommandSettings voiceCommandSettings)
        {
            using (var sw = new System.IO.StreamWriter(filename))
            {
                var serlizer = new System.Xml.Serialization.XmlSerializer(typeof(VoiceCommandSettings));
                serlizer.Serialize(sw, voiceCommandSettings);
            }
        }
    }
}
