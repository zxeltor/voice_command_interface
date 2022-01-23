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
    public partial class HotKeyPicker : Window
    {
        #region Fields

        private bool _isInEditMode;

        #endregion

        #region Constructors and Destructors

        public HotKeyPicker()
        {
            this.InitializeComponent();
        }

        #endregion

        #region Public Properties

        public Key HotKey { get; set; }

        #endregion

        #region Methods

        private void ToggleRecordOp(bool enable)
        {
            if (enable)
            {
                this.uiButRecord.Foreground = new SolidColorBrush(Colors.Red);
                this.uiButRecord.Content = "Recording";
                this.uiButRecord.FontWeight = FontWeights.Bold;
                this.uiTextBlockRecord.Foreground = new SolidColorBrush(Colors.Red);
                this.uiButRecord.KeyDown += this.uiTextBlockRecord_KeyDown;
                this.uiButRecord.KeyUp += this.uiTextBlockRecord_KeyUp;
                this.uiTextBlockRecord.KeyDown += this.uiTextBlockRecord_KeyDown;
                this.uiTextBlockRecord.KeyUp += this.uiTextBlockRecord_KeyUp;
            }
            else
            {
                this.uiButRecord.Foreground = new SolidColorBrush(Colors.Black);
                this.uiButRecord.Content = "Record";
                this.uiButRecord.FontWeight = FontWeights.Normal;
                this.uiTextBlockRecord.Foreground = new SolidColorBrush(Colors.Black);
                this.uiButRecord.KeyDown -= this.uiTextBlockRecord_KeyDown;
                this.uiButRecord.KeyUp -= this.uiTextBlockRecord_KeyUp;
                this.uiTextBlockRecord.KeyDown -= this.uiTextBlockRecord_KeyDown;
                this.uiTextBlockRecord.KeyUp -= this.uiTextBlockRecord_KeyUp;
            }
        }

        private void uiButApply_Click(object sender, RoutedEventArgs e)
        {
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
            //if (this._hotKeysList.Any())
            //{
            //    var lastKey = this._hotKeysList.Last();
            //    if (lastKey == e.Key)
            //        return;
            //}

            //this._hotKeysList.Add(e.Key);

            //var result = string.Empty;

            //for (var intI = 0; intI < this._hotKeysList.Count; intI++)
            //{
            //    if (intI < this._hotKeysList.Count - 1)
            //        result = result + this._hotKeysList[intI] + "+";
            //    else
            //        result = result + this._hotKeysList[intI];
            //}

            if (e.Key == Key.System)
                this.HotKey = e.SystemKey;
            else
                this.HotKey = e.Key;

            this.uiTextBlockRecord.Text = e.Key.ToString();
        }

        private void uiTextBlockRecord_KeyUp(object sender, KeyEventArgs e)
        {
            this.uiButRecord_Click(sender, e);
        }

        #endregion
    }

    //public class HotKeyInfo
    //{
    //    #region Constructors and Destructors

    //    public HotKeyInfo()
    //    {
    //        this.Keys = new List<Key>();
    //    }

    //    #endregion

    //    #region Public Properties

    //    public List<Key> Keys { get; set; }

    //    #endregion

    //    #region Public Methods and Operators

    //    public override string ToString()
    //    {
    //        var result = string.Empty;

    //        for (var intI = 0; intI < this.Keys.Count; intI++)
    //        {
    //            if (intI < this.Keys.Count - 1)
    //                result = result + this.Keys[intI] + "+";
    //            else
    //                result = result + this.Keys[intI];
    //        }

    //        return result;
    //    }

    //    #endregion
    //}
}