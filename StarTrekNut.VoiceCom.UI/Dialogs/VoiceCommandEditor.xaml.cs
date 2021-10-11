using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using StarTrekNut.VoiceCom.Lib.Classes;
using StarTrekNut.VoiceCom.Lib.Model.VoiceComSettings;
using System.Collections.ObjectModel;

namespace StarTrekNut.VoiceCom.UI.Dialogs
{
    /// <summary>
    ///     Interaction logic for ExePicker.xaml
    /// </summary>
    public partial class VoiceCommandEditor : Window
    {
        private bool _isInEditMode;
        private ObservableCollection<VoiceCommand> _voiceCommandList;
        private ObservableCollection<KeyTranslation> _keyTranslations;
        private EditorType _editorType;

        #region Constructors and Destructors

        public VoiceCommandEditor(
            EditorType editorType, 
            ObservableCollection<VoiceCommand> voiceCommandList,
            ObservableCollection<KeyTranslation> keyTranslations
            )
        {
            this.InitializeComponent();
            this._editorType = editorType;
            this._voiceCommandList = voiceCommandList;
            this._keyTranslations = keyTranslations;
        }

        #endregion

        #region Public Properties
        private VoiceCommand _voiceCommand;
        public VoiceCommand VoiceCommand
        {
            get
            {
                return this._voiceCommand;
            }
            set
            {
                this._voiceCommand = value;
                this.uiTextBoxGrammer.Text = value.Grammer;
                this.uiTextBoxKeyStrokes.Text = value.KeyStrokes;
            }
        }
        #endregion

        #region Methods

        private void uiButApply_Click(object sender, RoutedEventArgs e)
        {
            if (this.VoiceCommand == null)
            {
                MessageBox.Show("No voice command has been set.", "Voice Command Interface: Application Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (string.IsNullOrWhiteSpace(uiTextBoxGrammer.Text))
            {
                MessageBox.Show("You can't save a voice command with an empty voice command box.", "Voice Command Interface: Application Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (string.IsNullOrWhiteSpace(uiTextBoxKeyStrokes.Text))
            {
                MessageBox.Show("You can save a voice command with now keystroke(s), but it won't do anything.", "Voice Command Interface: Application Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }

            if(this._editorType == EditorType.Add || this._editorType == EditorType.Copy)
            {
                if (_voiceCommandList.Any(com => com.Grammer.Equals(uiTextBoxGrammer.Text, System.StringComparison.CurrentCultureIgnoreCase)))
                {
                    MessageBox.Show("A copy of this voice command already exists in your current profile.", "Voice Command Interface: Application Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
            }
            else
            {
                var voiceCommand = _voiceCommandList.FirstOrDefault(com => com.Grammer.Equals(uiTextBoxGrammer.Text, System.StringComparison.CurrentCultureIgnoreCase));
                if(voiceCommand != null)
                {
                    if(voiceCommand != this.VoiceCommand)
                    {
                        MessageBox.Show("A copy of this voice command already exists in your current profile.", "Voice Command Interface: Application Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }
                }
            }

            this.VoiceCommand.Grammer = string.IsNullOrWhiteSpace(uiTextBoxGrammer.Text) ? string.Empty : uiTextBoxGrammer.Text;
            this.VoiceCommand.KeyStrokes = string.IsNullOrWhiteSpace(uiTextBoxKeyStrokes.Text) ? string.Empty : uiTextBoxKeyStrokes.Text;

            this.DialogResult = true;
        }

        private void uiButCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        private void ToggleRecordOp(bool enable)
        {
            if (enable)
            {
                this.uiButRecord.Foreground = new SolidColorBrush(Colors.Red);
                this.uiButRecord.Content = "Recording ...";
                this.uiButRecord.FontWeight = FontWeights.Bold;
                this.uiTextBoxKeyStrokes.Foreground = new SolidColorBrush(Colors.Red);
                this.uiButRecord.PreviewKeyDown += this.uiTextBlockRecord_KeyDown;
                this.uiTextBoxKeyStrokes.PreviewKeyDown += this.uiTextBlockRecord_KeyDown;
            }
            else
            {
                this.uiButRecord.Foreground = new SolidColorBrush(Colors.Black);
                this.uiButRecord.Content = "Record Keybind";
                this.uiButRecord.FontWeight = FontWeights.Normal;
                this.uiTextBoxKeyStrokes.Foreground = new SolidColorBrush(Colors.Black);
                this.uiButRecord.PreviewKeyDown -= this.uiTextBlockRecord_KeyDown;
                this.uiTextBoxKeyStrokes.PreviewKeyDown -= this.uiTextBlockRecord_KeyDown;
            }
        }

        private void uiButRecord_Click(object sender, RoutedEventArgs e)
        {
            this._isInEditMode = !this._isInEditMode;
            this.ToggleRecordOp(this._isInEditMode);
        }

        private void uiTextBlockRecord_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.LeftShift || e.Key == Key.RightShift)
            {
                return;
            }
            if (e.Key == Key.LeftCtrl || e.Key == Key.RightCtrl)
            {
                return;
            }
            if (e.Key == Key.LeftAlt || e.Key == Key.RightAlt)
            {
                return;
            }

            this.uiTextBoxKeyStrokes.Text = KeyTranslation.GetSendKeysString(
                Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift),
                Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl),
                Keyboard.IsKeyDown(Key.LeftAlt) || Keyboard.IsKeyDown(Key.RightAlt),
                e.Key,
                _keyTranslations);

            this.uiButRecord_Click(sender, e);
        }
        #endregion
    }

    public enum EditorType
    {
        Add,
        Edit,
        Copy
    }        
}