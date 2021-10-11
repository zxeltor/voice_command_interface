using System.ComponentModel;

namespace StarTrekNut.VoiceCom.Lib.Model
{
    public class VoiceCommand : INotifyPropertyChanged
    {
        private string _grammer;

        private string _keystrokes;

        /// <summary>
        /// A voice command for speech recognition.
        /// </summary>
        public string Grammer
        {
            get { return _grammer; }
            set
            {
                _grammer = value;
                NotifyPropertyChange("Grammer");
            }
        }

        /// <summary>
        /// The resulting keystrokes sent to the target application in response to the voice command
        /// </summary>
        public string KeyStrokes
        {
            get { return _keystrokes; }
            set
            {
                _keystrokes = value;
                NotifyPropertyChange("KeyStrokes");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Standard ToString() override
        /// </summary>
        /// <returns>Returns a string with both the Grammer and KeyStroke strings concatenated "Grammer={Grammer}, KeyStrokes={KeyStrokes}"</returns>
        public override string ToString()
        {
            return $"Grammer={Grammer}, KeyStrokes={KeyStrokes}";
        }

        private void NotifyPropertyChange(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// A deep copy clone of the object
        /// </summary>
        /// <returns></returns>
        public VoiceCommand Clone()
        {
            return new VoiceCommand {Grammer = Grammer, KeyStrokes = KeyStrokes};
        }
    }
}