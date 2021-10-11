using System;
using System.Diagnostics;
using System.Linq;
using System.Windows;

namespace StarTrekNut.VoiceCom.UI.Dialogs
{
    /// <summary>
    ///     Interaction logic for ExePicker.xaml
    /// </summary>
    public partial class ExePicker : Window
    {
        #region Constructors and Destructors

        public ExePicker()
        {
            this.InitializeComponent();

            this.Loaded += this.ExePicker_Loaded;
        }

        #endregion

        #region Public Properties

        public string SelectedProcessMainWindowTitle { get; set; }

        public string SelectedProcessName { get; set; }

        #endregion

        #region Methods

        private void ExePicker_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                var processes = Process.GetProcesses().ToList().OrderBy(p => p.ProcessName);
                foreach (var p in processes)
                {
                    if (!string.IsNullOrEmpty(p.MainWindowTitle))
                        this.uiListViewProcs.Items.Add(new MyProcInfo(p));
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(
                    $"Unable to get a list of running applications.\n\nAdditional Error Info: {exception.Message}",
                    "Voice Command Interface: Application Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private void uiButApply_Click(object sender, RoutedEventArgs e)
        {
            var proc = this.uiListViewProcs.SelectedItem as MyProcInfo;
            if (!string.IsNullOrWhiteSpace(proc?.ProcessName))
            {
                this.SelectedProcessName = proc.ProcessName;
                this.SelectedProcessMainWindowTitle = proc.MainWindowTitle;
            }

            this.DialogResult = true;
        }

        private void uiButCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        #endregion
    }

    public class MyProcInfo
    {
        #region Constructors and Destructors

        public MyProcInfo(Process process)
        {
            this.Process = process;
        }

        #endregion

        #region Public Properties

        public string MainWindowTitle => this.Process.MainWindowTitle.Length > 80 ? this.Process.MainWindowTitle.Substring(0, 80) : this.Process.MainWindowTitle;

        public Process Process { get; set; }

        public string ProcessName => this.Process.ProcessName;

        #endregion
    }
}