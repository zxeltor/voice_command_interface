using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using StarTrekNut.VoiceCom.Lib.Classes;
using StarTrekNut.VoiceCom.Lib.Model.VoiceComSettings;
using System.Collections.ObjectModel;
using System;

namespace StarTrekNut.VoiceCom.UI.Dialogs
{
    /// <summary>
    ///     Interaction logic for ExePicker.xaml
    /// </summary>
    public partial class KeyTranslationEditor : Window
    {
        private bool _isInEditMode;
        private ObservableCollection<KeyTranslation> _keyTranslations;
        private EditorType _editorType;

        #region Constructors and Destructors

        public KeyTranslationEditor(
            EditorType editorType,
            ObservableCollection<KeyTranslation> keyTranslations
            )
        {
            this.InitializeComponent();
            this._editorType = editorType;
            this._keyTranslations = keyTranslations;
        }

        #endregion

        #region Public Properties
        private KeyTranslation _keyTranslation;
        public KeyTranslation KeyTranslation
        {
            get
            {
                return this._keyTranslation;
            }
            set
            {
                this._keyTranslation = value;
                this.uiTextBoxKey.Text = value.Key.ToString();
                this.uiTextBoxTranslation.Text = value.Translation;
            }
        }
        #endregion

        #region Methods

        private void uiButApply_Click(object sender, RoutedEventArgs e)
        {
            //if (this.VoiceCommand == null)
            //{
            //    MessageBox.Show("No voice command has been set.", "Voice Command Interface: Application Error", MessageBoxButton.OK, MessageBoxImage.Error);
            //    return;
            //}

            if (string.IsNullOrWhiteSpace(uiTextBoxKey.Text))
            {
                MessageBox.Show("You can't save a key translation without selecting a key.", "Voice Command Interface: Application Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (string.IsNullOrWhiteSpace(uiTextBoxTranslation.Text))
            {
                MessageBox.Show("You can't save a key translation without selecting a translation.", "Voice Command Interface: Application Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if(this._editorType == EditorType.Add || this._editorType == EditorType.Copy)
            {
                if (_keyTranslations.Any(com => com.Key.ToString().Equals(uiTextBoxKey.Text, System.StringComparison.CurrentCultureIgnoreCase)))
                {
                    MessageBox.Show("A copy of this key translation already exists for the current application.", "Voice Command Interface: Application Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
            }
            else
            {
                var keyTranslation = _keyTranslations.FirstOrDefault(com => com.Key.ToString().Equals(uiTextBoxKey.Text, System.StringComparison.CurrentCultureIgnoreCase));
                if(keyTranslation != null)
                {
                    if(keyTranslation != this.KeyTranslation)
                    {
                        MessageBox.Show("A copy of this key translation already exists for the current application.", "Voice Command Interface: Application Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }
                }
            }

            Key keyTmp;
            if(!Enum.TryParse<Key>(uiTextBoxKey.Text, out keyTmp))
            {
                MessageBox.Show("Couldn't process the key you selected.", "Voice Command Interface: Application Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            
            this.KeyTranslation.Key = keyTmp;
            this.KeyTranslation.Translation = string.IsNullOrWhiteSpace(uiTextBoxTranslation.Text) ? string.Empty : uiTextBoxTranslation.Text;

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
                this.uiTextBoxKey.Foreground = new SolidColorBrush(Colors.Red);
                this.uiButRecord.KeyDown += this.uiTextBlockRecord_KeyDown;
                this.uiTextBoxKey.KeyDown += this.uiTextBlockRecord_KeyDown;
            }
            else
            {
                this.uiButRecord.Foreground = new SolidColorBrush(Colors.Black);
                this.uiButRecord.Content = "Record Key";
                this.uiButRecord.FontWeight = FontWeights.Normal;
                this.uiTextBoxKey.Foreground = new SolidColorBrush(Colors.Black);
                this.uiButRecord.KeyDown -= this.uiTextBlockRecord_KeyDown;
                this.uiTextBoxKey.KeyDown -= this.uiTextBlockRecord_KeyDown;
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

            //this.uiTextBoxKeyStrokes.Text = KeyTranslation.GetSendKeysString(
            //    Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift),
            //    Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl),
            //    Keyboard.IsKeyDown(Key.LeftAlt) || Keyboard.IsKeyDown(Key.RightAlt),
            //    e.Key,
            //    _keyTranslations);

            this.uiTextBoxKey.Text = e.Key.ToString();

            this.uiButRecord_Click(sender, e);
        }
        #endregion
    }
}