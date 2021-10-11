using System.Collections.ObjectModel;
using System.ComponentModel;

namespace StarTrekNut.VoiceCom.Lib.Model.VoiceComSettings
{
    public class User : INotifyPropertyChanged
    {
        #region Fields

        private ObservableCollection<Executable> _executableList;

        private ObservableCollection<HotKey> _hotKeysList;

        private string _startupExecutableName;

        private SpeechRecogSettings _startupSpeechRecogSettings;

        private TtsSettings _startupTtsSettings;

        private string _userName;

        private bool _enableSoftwareUpdates;

        #endregion

        #region Constructors and Destructors

        public User()
        {
            this.StartupTtsSettings = new TtsSettings();
            this.StartupSpeechRecogSettings = new SpeechRecogSettings();
            this.HotKeysList = new ObservableCollection<HotKey>();
            this.ExecutableList = new ObservableCollection<Executable>();
        }

        #endregion

        #region Public Events

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Public Properties

        public ObservableCollection<Executable> ExecutableList
        {
            get => this._executableList;
            set
            {
                this._executableList = value;
                this.NotifyPropertyChange("ExecutableList");
            }
        }

        public ObservableCollection<HotKey> HotKeysList
        {
            get => this._hotKeysList;
            set
            {
                this._hotKeysList = value;
                this.NotifyPropertyChange("HotKeysList");
            }
        }

        public string StartupExecutableName
        {
            get => this._startupExecutableName;
            set
            {
                this._startupExecutableName = value;
                this.NotifyPropertyChange("StartupExecutableName");
            }
        }

        public SpeechRecogSettings StartupSpeechRecogSettings
        {
            get => this._startupSpeechRecogSettings;
            set
            {
                this._startupSpeechRecogSettings = value;
                this.NotifyPropertyChange("StartupSpeechRecogSettings");
            }
        }

        public TtsSettings StartupTtsSettings
        {
            get => this._startupTtsSettings;
            set
            {
                this._startupTtsSettings = value;
                this.NotifyPropertyChange("StartupTtsSettings");
            }
        }

        public string UserName
        {
            get => this._userName;
            set
            {
                this._userName = value;
                this.NotifyPropertyChange("UserName");
            }
        }

        public bool EnableSoftwareUpdates
        {
            get => this._enableSoftwareUpdates;
            set
            {
                this._enableSoftwareUpdates = value;
                this.NotifyPropertyChange("EnableSoftwareUpdates");
            }
        }

        #endregion

        #region Public Methods and Operators

        public static User GetDefaultForUser()
        {
            return new User { StartupTtsSettings = new TtsSettings { SelectedVolume = 50, EnableTtsCommandAck = true } };
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