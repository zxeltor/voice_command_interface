using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using StarTrekNut.VoiceCom.Lib.Classes;
using StarTrekNut.VoiceCom.Lib.Helpers;
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
        private EditorType _editorType;
        private List<Key> _keyStrokes = new List<Key>();

        #region Constructors and Destructors

        public VoiceCommandEditor(
            EditorType editorType, 
            ObservableCollection<VoiceCommand> voiceCommandList
            )
        {
            this.InitializeComponent();
            this._editorType = editorType;
            this._voiceCommandList = voiceCommandList;
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
                this.uiTextBoxKeyStrokes.Text = KeyTranslationHelper.GetVisualKeyString(value.KeyStrokes); //value.KeyStrokesString;
                this._keyStrokes = value.KeyStrokes;
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
                MessageBox.Show("You can save a voice command with no keystroke(s), but it won't do anything.", "Voice Command Interface: Application Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
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
            this.VoiceCommand.KeyStrokes = _keyStrokes;
            
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
                this.uiButRecord.Content = "Recording";
                this.uiButRecord.ToolTip = "Click to STOP recording keystrokes.";
                this.uiButRecord.FontWeight = FontWeights.Bold;
                this.uiTextBoxKeyStrokes.Foreground = new SolidColorBrush(Colors.Red);
                this.uiButRecord.PreviewKeyDown += this.uiTextBlockRecord_KeyDown;
                this.uiTextBoxKeyStrokes.PreviewKeyDown += this.uiTextBlockRecord_KeyDown;
                //this.uiTextBoxKeyStrokes.Text = string.Empty;
                //this._keyStrokes.Clear();
            }
            else
            {
                this.uiButRecord.Foreground = new SolidColorBrush(Colors.Black);
                this.uiButRecord.Content = "Record";
                this.uiButRecord.ToolTip = "Click to START recording keystrokes.";
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

        //bool LeftAltKeyPressed = false;
        //bool RightAltKeyPressed = false;
        private void uiTextBlockRecord_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.IsRepeat)
            {
                e.Handled = true;
                return;
            }
            else if(e.Key == Key.LeftShift || e.Key == Key.RightShift || e.Key == Key.LeftCtrl || 
                e.Key == Key.RightCtrl || e.Key == Key.LeftAlt || e.Key == Key.RightAlt)
            {
                this.uiTextBoxKeyStrokes.Text += string.IsNullOrWhiteSpace(this.uiTextBoxKeyStrokes.Text) ? e.Key.ToString() : $"+{e.Key}";
                this._keyStrokes.Add(e.Key);

                e.Handled = true;
                return;
            }
            else if(e.Key == Key.System)
            {
                this.uiTextBoxKeyStrokes.Text += string.IsNullOrWhiteSpace(this.uiTextBoxKeyStrokes.Text) ? e.SystemKey.ToString() : $"+{e.SystemKey}";
                this._keyStrokes.Add(e.SystemKey);

                e.Handled = true;
                return;
            }

            this.uiTextBoxKeyStrokes.Text += string.IsNullOrWhiteSpace(this.uiTextBoxKeyStrokes.Text) ? e.Key.ToString() : $"+{e.Key}";
            this._keyStrokes.Add(e.Key);

            e.Handled = true;
        }
        #endregion

        private void uiButClearKeyStrokes_Click(object sender, RoutedEventArgs e)
        {
            this._isInEditMode = false;
            this.ToggleRecordOp(this._isInEditMode);

            this.uiTextBoxKeyStrokes.Text = string.Empty;
            this._keyStrokes.Clear();
        }
    }

    public enum EditorType
    {
        Add,
        Edit,
        Copy
    }        
}