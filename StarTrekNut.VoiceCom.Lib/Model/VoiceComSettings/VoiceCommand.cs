using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;

namespace StarTrekNut.VoiceCom.Lib.Model.VoiceComSettings
{
    /// <summary>
    ///     Class used to save settings, and for databinding in the app.
    /// </summary>
    public class VoiceCommand : INotifyPropertyChanged
    {
        #region Fields

        private string _grammer;

        private List<System.Windows.Input.Key> _keyStrokes = new List<System.Windows.Input.Key>();

        private bool _enabled = true;

        #endregion

        #region Public Events

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Public Properties

        public string Grammer
        {
            get => this._grammer;
            set
            {
                this._grammer = value;
                this.NotifyPropertyChange("Grammer");
            }
        }

        public List<System.Windows.Input.Key> KeyStrokes
        {
            get => this._keyStrokes;
            set
            {
                this._keyStrokes = value;
                this.NotifyPropertyChange("KeyStrokes");
            }
        }

        public bool Enabled
        {
            get => this._enabled;
            set
            {
                this._enabled = value;
                this.NotifyPropertyChange("Enabled");
            }
        }

        #endregion

        #region Methods

        private void NotifyPropertyChange(string propertyName)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        public override string ToString()
        {
            return $"{this.Enabled}|{this.Grammer}|{string.Join("+", this.KeyStrokes.ToList())}";
        }
    }
}