using System.Collections.ObjectModel;
using System.ComponentModel;

namespace StarTrekNut.VoiceCom.Lib.Model.VoiceComSettings
{
    /// <summary>
    ///     Class used to save settings, and for databinding in the app.
    /// </summary>
    public class Executable : INotifyPropertyChanged
    {
        #region Fields

        private string _executableName;

        private ObservableCollection<ProfileSettings> _profileSettingsList;

        private string _startupProfileName;

        #endregion

        #region Constructors and Destructors

        public Executable()
        {
            this.ProfileSettingsList = new ObservableCollection<ProfileSettings>();
        }

        #endregion

        #region Public Events

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Public Properties

        public string ExecutableName
        {
            get => this._executableName;
            set
            {
                this._executableName = value;
                this.NotifyPropertyChange("ExecutableName");
            }
        }
                
        public ObservableCollection<ProfileSettings> ProfileSettingsList
        {
            get => this._profileSettingsList;
            set
            {
                this._profileSettingsList = value;
                this.NotifyPropertyChange("ProfileSettingsList");
            }
        }

        public string StartupProfileName
        {
            get => this._startupProfileName;
            set
            {
                this._startupProfileName = value;
                this.NotifyPropertyChange("StartupProfileName");
            }
        }
        #endregion

        #region Public Methods and Operators

        public override string ToString()
        {
            return this.ExecutableName;
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