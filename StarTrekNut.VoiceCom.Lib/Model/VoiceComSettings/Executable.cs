using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Collections.Generic;
using System.Windows.Input;

namespace StarTrekNut.VoiceCom.Lib.Model.VoiceComSettings
{
    public class Executable : INotifyPropertyChanged
    {
        #region Fields

        private string _executableName;

        private ObservableCollection<ProfileSettings> _profileSettingsList;

        private ObservableCollection<KeyTranslation> _keyTranslations;

        private string _startupProfileName;

        #endregion

        #region Constructors and Destructors

        public Executable()
        {
            this.ProfileSettingsList = new ObservableCollection<ProfileSettings>();
            this.KeyTranslations = new ObservableCollection<KeyTranslation>();
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

        public ObservableCollection<KeyTranslation> KeyTranslations
        {
            get => this._keyTranslations;
            set
            {
                this._keyTranslations = value;
                this.NotifyPropertyChange("KeyTranslations");
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