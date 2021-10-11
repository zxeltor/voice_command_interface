using System.ComponentModel;
using StarTrekNut.VoiceCom.Lib;

namespace StarTrekNut.VoiceCom.UI.Classes
{
    public class VoiceCommandDataContext : INotifyPropertyChanged
    {
        private VoiceComSettings _appSettings;

        private ClientMessageLogger _clientLogger;

        private SpeechProcessor _speechProcessor;

        public SpeechProcessor SpeechProc
        {
            get
            {
                if (_speechProcessor == null)
                {
                    _speechProcessor = SpeechProcessor.Instance;
                    NotifyPropertyChange("SpeechProc");
                }
                return _speechProcessor;
            }
        }

        public VoiceComSettings Settings => VoiceComSettings.Instance;

        public ClientMessageLogger ClientLogger
        {
            get
            {
                if (_clientLogger == null)
                {
                    _clientLogger = ClientMessageLogger.Instance;
                    NotifyPropertyChange("ClientLogger");
                }
                return _clientLogger;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChange(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}