using System.Collections.ObjectModel;
using System.ComponentModel;

namespace StarTrekNut.VoiceCom.Lib.Model.VoiceComSettings
{
    /// <summary>
    ///     Class used to save settings, and for databinding in the app.
    /// </summary>
    public class User : INotifyPropertyChanged
    {
        #region Fields

        private ObservableCollection<Executable> _executableList;

        private ObservableCollection<System.Windows.Input.Key> _hotKeysList;

        private string _startupExecutableName;

        private SpeechRecogSettings _startupSpeechRecogSettings;

        private TtsSettings _startupTtsSettings;

        private bool _enableSoftwareUpdates;

        private int _keystrokeDelayInMilliSeconds = 150;

        #endregion

        #region Constructors and Destructors

        public User()
        {
            this.StartupTtsSettings = new TtsSettings();
            this.StartupSpeechRecogSettings = new SpeechRecogSettings();
            this.HotKeysList = new ObservableCollection<System.Windows.Input.Key>();
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

        public ObservableCollection<System.Windows.Input.Key> HotKeysList
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

        public int SelectedKeystrokeDelayInMilliSeconds
        {
            get => this._keystrokeDelayInMilliSeconds;
            set
            {
                this._keystrokeDelayInMilliSeconds = value;
                this.NotifyPropertyChange("SelectedKeystrokeDelayInMilliSeconds");
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