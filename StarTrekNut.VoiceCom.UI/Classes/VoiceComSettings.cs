using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows.Input;
using System.Xml.Serialization;
using StarTrekNut.VoiceCom.Lib.Model;

namespace StarTrekNut.VoiceCom.UI.Classes
{
    /// <summary>
    /// A change to test how the branch stuff works.
    /// </summary>
    public class VoiceComSettings : INotifyPropertyChanged
    {
        private static VoiceComSettings _instance;

        private ObservableCollection<CharacterProfileSettings> _characterProfileSettingsList;

        private string _defaultTtsVoice;

        private bool _enableCommandAck;

        private ObservableCollection<Key> _hotkeys;

        private CharacterProfileSettings _selectedCharacterProfileSetting;

        private string _warcraftExecutableName;

        private VoiceComSettings()
        {
        }

        [XmlIgnore]
        public static VoiceComSettings Instance
        {
            get
            {
                if(_instance == null) GetWowVoiceComSettingsFromFile();
                return _instance;
            }
        }

        public string DefaultTtsVoice
        {
            get { return _defaultTtsVoice; }
            set
            {
                _defaultTtsVoice = value;
                NotifyPropertyChange("DefaultTtsVoice");
            }
        }

        //public string WarcraftExecutableName
        //{
        //    get { return _warcraftExecutableName; }
        //    set
        //    {
        //        _warcraftExecutableName = value;
        //        NotifyPropertyChange("WarcraftExecutableName");
        //    }
        //}

        //public bool EnableCommandAck
        //{
        //    get { return _enableCommandAck; }
        //    set
        //    {
        //        _enableCommandAck = value;
        //        NotifyPropertyChange("EnableCommandAck");
        //    }
        //}

        public ObservableCollection<CharacterProfileSettings> CharacterProfileSettingsList
        {
            get { return _characterProfileSettingsList; }
            set
            {
                _characterProfileSettingsList = value;
                NotifyPropertyChange("CharacterProfileSettingsList");
            }
        }

        [XmlIgnore]
        public CharacterProfileSettings SelectedCharacterProfileSettings
        {
            get { return _selectedCharacterProfileSetting; }
            set
            {
                _selectedCharacterProfileSetting = value;
                NotifyPropertyChange("SelectedCharacterProfileSettings");
            }
        }

        public ObservableCollection<Key> Hotkeys
        {
            get { return _hotkeys; }
            set
            {
                _hotkeys = value;
                NotifyPropertyChange("Hotkeys");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChange(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void SaveWowVoiceComSettingsToFile()
        {
            var xmlSerializer = new System.Xml.Serialization.XmlSerializer(typeof(VoiceComSettings));
            using (var sw = new System.IO.StreamWriter("VoiceComSettings.xml"))
            {
                xmlSerializer.Serialize(sw, this);
            }
        }

        public static void GetWowVoiceComSettingsFromFile()
        {
            var xmlSerializer = new System.Xml.Serialization.XmlSerializer(typeof(VoiceComSettings));
            using (var sr = new System.IO.StreamReader("VoiceComSettings.xml"))
            {
                _instance = (VoiceComSettings)xmlSerializer.Deserialize(sr);
            }
        }
    }
}