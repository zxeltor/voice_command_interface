using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Xml.Serialization;

namespace StarTrekNut.VoiceCom.Lib.Model
{
    /// <summary>
    /// A proxy class used to represent a character settings xml node.
    /// </summary>
    public class CharacterProfileSettings : INotifyPropertyChanged
    {
        private string _charName;

        /// <summary>
        /// The name of the character or profile name
        /// </summary>
        [XmlAttribute]
        public string CharacterName
        {
            get { return _charName; }
            set
            {
                _charName = value;
                NotifyPropertyChange("CharacterName");
            }
        }

        /// <summary>
        /// A collection of voice commands and their keystrokes
        /// </summary>
        public ObservableCollection<VoiceCommand> VoiceCommands { get; set; }

        /// <summary>
        /// Property change event handler
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Standard ToString() override
        /// </summary>
        /// <returns>Returns the CharacterName property</returns>
        public override string ToString()
        {
            return CharacterName;
        }

        /// <summary>
        /// Create a deep copy clone of this objects KeyStrokes
        /// </summary>
        /// <returns>A deep copy clone of this objects KeyStrokes</returns>
        public ObservableCollection<VoiceCommand> CloneKeyStrokes()
        {
            var clonedKeys = VoiceCommands.Select(key => key.Clone());
            return new ObservableCollection<VoiceCommand>(clonedKeys);
        }

        /// <summary>
        /// Helper method to send notification events
        /// </summary>
        /// <param name="propertyName"></param>
        private void NotifyPropertyChange(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}