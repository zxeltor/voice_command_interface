using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace StarTrekNut.VoiceCom.Lib.Model.VoiceComSettings
{
    public class ProfileSettings : INotifyPropertyChanged
    {
        #region Fields

        private string _profileName;

        private ObservableCollection<VoiceCommand> _voiceCommandList;

        #endregion

        #region Constructors and Destructors

        public ProfileSettings()
        {
            this.VoiceCommandList = new ObservableCollection<VoiceCommand>();
        }

        #endregion

        #region Public Events

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Public Properties

        public string ProfileName
        {
            get => this._profileName;
            set
            {
                this._profileName = value;
                this.NotifyPropertyChange("ProfileName");
            }
        }

        public ObservableCollection<VoiceCommand> VoiceCommandList
        {
            get => this._voiceCommandList;
            set
            {
                this._voiceCommandList = value;
                this.NotifyPropertyChange("VoiceCommandList");
            }
        }

        #endregion

        #region Public Methods and Operators

        public ObservableCollection<VoiceCommand> CloneVoiceCommands()
        {
            var clonedList = this.VoiceCommandList.Select(vc => new VoiceCommand {Enabled = vc.Enabled,  Grammer = vc.Grammer, KeyStrokes = vc.KeyStrokes });
            return new ObservableCollection<VoiceCommand>(clonedList);
        }

        public override string ToString()
        {
            return $"{this.ProfileName}";
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