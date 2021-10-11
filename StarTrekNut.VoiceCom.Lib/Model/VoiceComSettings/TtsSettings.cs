using System.ComponentModel;

namespace StarTrekNut.VoiceCom.Lib.Model.VoiceComSettings
{
    public class TtsSettings : INotifyPropertyChanged
    {
        #region Fields

        private bool _enableTtsCommandAck;

        private string _selectedVoice;

        private int _selectedVolume;

        #endregion

        #region Constructors and Destructors

        public TtsSettings()
        {
            this.SelectedVolume = 50;
            this.EnableTtsCommandAck = false;
        }

        #endregion

        #region Public Events

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Public Properties

        public bool EnableTtsCommandAck
        {
            get => this._enableTtsCommandAck;
            set
            {
                this._enableTtsCommandAck = value;
                this.NotifyPropertyChange("EnableTtsCommandAck");
            }
        }

        public string SelectedVoice
        {
            get => this._selectedVoice;
            set
            {
                this._selectedVoice = value;
                this.NotifyPropertyChange("SelectedVoice");
            }
        }

        public int SelectedVolume
        {
            get => this._selectedVolume;
            set
            {
                this._selectedVolume = value;
                this.NotifyPropertyChange("SelectedVolume");
            }
        }

        #endregion

        #region Methods

        private void NotifyPropertyChange(string propertyName)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}