using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace StarTrekNut.VoiceCom.UI.Dialogs
{
    /// <summary>
    ///     Interaction logic for ExePicker.xaml
    /// </summary>
    public partial class KeystrokePicker : Window
    {
        #region Fields

        private readonly List<Key> _controlKeysList = new List<Key>();

        private bool _isInEditMode;

        #endregion

        #region Constructors and Destructors

        public KeystrokePicker()
        {
            this.InitializeComponent();
        }

        #endregion

        #region Public Properties

        public HotKeyInfo HotKey { get; set; }

        #endregion

        #region Methods

        private void ToggleRecordOp(bool enable)
        {
            if (enable)
            {
                this.uiButRecord.Foreground = new SolidColorBrush(Colors.Red);
                this.uiButRecord.Content = "Recording ...";
                this.uiButRecord.FontWeight = FontWeights.Bold;
                this.uiTextKeystrokes.Foreground = new SolidColorBrush(Colors.Red);
                this.uiButRecord.KeyDown += this.uiTextBlockRecord_KeyDown;
                this.uiButRecord.KeyUp += this.uiTextBlockRecord_KeyUp;
                this.uiTextKeystrokes.KeyDown += this.uiTextBlockRecord_KeyDown;
                this.uiTextKeystrokes.KeyUp += this.uiTextBlockRecord_KeyUp;

                this._controlKeysList.Clear();
            }
            else
            {
                this.uiButRecord.Foreground = new SolidColorBrush(Colors.Black);
                this.uiButRecord.Content = "Add Modified Keystroke";
                this.uiButRecord.FontWeight = FontWeights.Normal;
                this.uiTextKeystrokes.Foreground = new SolidColorBrush(Colors.Black);
                this.uiButRecord.KeyDown -= this.uiTextBlockRecord_KeyDown;
                this.uiButRecord.KeyUp -= this.uiTextBlockRecord_KeyUp;
                this.uiTextKeystrokes.KeyDown -= this.uiTextBlockRecord_KeyDown;
                this.uiTextKeystrokes.KeyUp -= this.uiTextBlockRecord_KeyUp;
            }
        }

        private void uiButApply_Click(object sender, RoutedEventArgs e)
        {
            this.HotKey = new HotKeyInfo { Keys = this._controlKeysList ?? new List<Key>() };

            this.DialogResult = true;
        }

        private void uiButCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        private void uiButRecord_Click(object sender, RoutedEventArgs e)
        {
            this._isInEditMode = !this._isInEditMode;
            this.ToggleRecordOp(this._isInEditMode);
        }

        private void uiTextBlockRecord_KeyDown(object sender, KeyEventArgs e)
        {
            if (this._controlKeysList.Any())
            {
                var lastKey = this._controlKeysList.Last();
                if (lastKey == e.Key)
                    return;
            }

            this._controlKeysList.Add(e.Key);

            var result = string.Empty;

            for (var intI = 0; intI < this._controlKeysList.Count; intI++)
            {
                if (intI < this._controlKeysList.Count - 1)
                    result = result + this._controlKeysList[intI] + "+";
                else
                    result = result + this._controlKeysList[intI];
            }

            this.uiTextKeystrokes.Text = this.uiTextKeystrokes.Text + result;
        }

        private void uiTextBlockRecord_KeyUp(object sender, KeyEventArgs e)
        {
            this.uiButRecord_Click(sender, e);
        }

        #endregion
    }

    public class VoiceCommandInfo
    {
        #region Constructors and Destructors

        public VoiceCommandInfo()
        {
            this.Keys = new List<Key>();
        }

        #endregion

        #region Public Properties

        public List<Key> Keys { get; set; }

        #endregion

        #region Public Methods and Operators

        public override string ToString()
        {
            var result = string.Empty;

            for (var intI = 0; intI < this.Keys.Count; intI++)
            {
                if (intI < this.Keys.Count - 1)
                    result = result + this.Keys[intI] + "+";
                else
                    result = result + this.Keys[intI];
            }

            return result;
        }

        #endregion
    }
}