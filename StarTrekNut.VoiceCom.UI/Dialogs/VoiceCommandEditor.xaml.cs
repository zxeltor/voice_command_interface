using System;
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
        private ObservableCollection<KeyTainer> _keyStrokes = new ObservableCollection<KeyTainer>();
                
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
                value.KeyStrokes.ForEach(key => this._keyStrokes.Add(new KeyTainer(key)));
                this.DataContext = this._keyStrokes;
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

            if (!this._keyStrokes.Any())
            {
                MessageBox.Show("You can save a voice command with no keystroke(s), but it won't do anything.", "Voice Command Interface: Application Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }

            if (this._editorType == EditorType.Add || this._editorType == EditorType.Copy)
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
            this.VoiceCommand.KeyStrokes = _keyStrokes.Select(key => key.WindowsKey).ToList();
            
            this.DialogResult = true;
        }

        private void uiButCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        private void ToggleRecordOp(object sender, bool enable)
        {
            var recordButton = sender as System.Windows.Controls.Button;
            if (recordButton == null) return;

            if (enable)
            {
                recordButton.Background = new SolidColorBrush(Colors.Red);
                recordButton.ToolTip = "Click to STOP recording keystrokes.";
                uiListViewKeystrokes.PreviewKeyDown += this.uiTextBlockRecord_KeyDown;
            }
            else
            {
                recordButton.Background = new SolidColorBrush(Colors.Transparent);
                recordButton.ToolTip = "Click to START recording keystrokes.";
                uiListViewKeystrokes.PreviewKeyDown -= this.uiTextBlockRecord_KeyDown;
            }
        }

        private void uiButRecord_Click(object sender, RoutedEventArgs e)
        {
            this._isInEditMode = !this._isInEditMode;
            this.ToggleRecordOp(sender, this._isInEditMode);
        }

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
                this._keyStrokes.Add(new KeyTainer(e.Key));

                e.Handled = true;
                return;
            }
            else if(e.Key == Key.System)
            {
                this._keyStrokes.Add(new KeyTainer(e.SystemKey));

                e.Handled = true;
                return;
            }

            this._keyStrokes.Add(new KeyTainer(e.Key));

            e.Handled = true;
        }
        #endregion

        private void uiButClearKeyStrokes_Click(object sender, RoutedEventArgs e)
        {
            this._isInEditMode = false;
            this._keyStrokes.Clear();
        }

        private void MoveKeyUpButton_Click(object sender, RoutedEventArgs e)
        {
            var upButton = sender as System.Windows.Controls.Button;
            if (upButton == null)
                return;

            var upButtonKeyContext = upButton.DataContext as KeyTainer;
            if (upButtonKeyContext == null)
                return;

            for(int intI=this._keyStrokes.Count-1; intI>0; intI--)
            {
                if(this._keyStrokes[intI].Id.Equals(upButtonKeyContext.Id))
                {
                    this._keyStrokes.RemoveAt(intI);
                    this._keyStrokes.Insert(--intI, upButtonKeyContext);
                    break;
                }
            }
        }

        private void MoveKeyDownButton_Click(object sender, RoutedEventArgs e)
        {
            var upButton = sender as System.Windows.Controls.Button;
            if (upButton == null)
                return;

            var upButtonKeyContext = upButton.DataContext as KeyTainer;
            if (upButtonKeyContext == null)
                return;

            for (int intI = 0; intI < this._keyStrokes.Count - 1; intI++)
            {
                if (this._keyStrokes[intI].Id.Equals(upButtonKeyContext.Id))
                {
                    this._keyStrokes.Insert(intI+2, upButtonKeyContext);
                    this._keyStrokes.RemoveAt(intI);
                    break;
                }
            }
        }

        private void DeleteKeyButton_Click(object sender, RoutedEventArgs e)
        {
            var upButton = sender as System.Windows.Controls.Button;
            if (upButton == null)
                return;

            var upButtonKeyContext = upButton.DataContext as KeyTainer;
            if (upButtonKeyContext == null)
                return;

            for (int intI = this._keyStrokes.Count - 1; intI >= 0; intI--)
            {
                if (this._keyStrokes[intI].Id.Equals(upButtonKeyContext.Id))
                {
                    this._keyStrokes.RemoveAt(intI);
                    break;
                }
            }
        }
    }

    public enum EditorType
    {
        Add,
        Edit,
        Copy
    }

    public class KeyTainer : IEquatable<KeyTainer>
    {
        public System.Guid Id { get; private set; }
        public Key WindowsKey { get; private set; }

        public KeyTainer(Key windowsKey)
        {
            this.Id = System.Guid.NewGuid();
            this.WindowsKey = windowsKey;
        }

        public bool Equals(KeyTainer other)
        {
            return this.Id.Equals(other.Id);
        }

        public override string ToString()
        {
            return $"{this.WindowsKey}|{this.Id}";
        }
    }
}