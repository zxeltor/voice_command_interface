using System.ComponentModel;

namespace StarTrekNut.VoiceCom.Lib.Model.VoiceComSettings
{
    public class SpeechRecogSettings : INotifyPropertyChanged
    {
        #region Fields

        private string _selectedEngine;

        #endregion

        #region Public Events

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Public Properties

        public string SelectedEngine
        {
            get => this._selectedEngine;
            set
            {
                this._selectedEngine = value;
                this.NotifyPropertyChange("SelectedEngine");
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