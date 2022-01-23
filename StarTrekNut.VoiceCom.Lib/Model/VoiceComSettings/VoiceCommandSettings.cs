using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Serialization;

namespace StarTrekNut.VoiceCom.Lib.Model.VoiceComSettings
{
    public class VoiceCommandSettings : INotifyPropertyChanged
    {
        #region Fields

        private List<User> _userList;

        #endregion

        #region Constructors and Destructors

        public VoiceCommandSettings()
        {
            this.UserList = new List<User>();
        }

        #endregion

        #region Public Events

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Public Properties

        public List<User> UserList
        {
            get => this._userList;
            set
            {
                this._userList = value;
                this.NotifyPropertyChange("UserList");
            }
        }
        
        [XmlIgnore]
        public bool IsNewFile { get; set; }

        #endregion
        
        #region Methods

        private void NotifyPropertyChange(string propertyName)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}