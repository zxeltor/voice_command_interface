using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

using StarTrekNut.VoiceCom.Lib;
using StarTrekNut.VoiceCom.Lib.Classes;
using StarTrekNut.VoiceCom.Lib.Model.VoiceComSettings;
using StarTrekNut.VoiceCom.UI.Classes;
using StarTrekNut.VoiceCom.UI.Dialogs;
using StarTrekNut.VoiceCom.UI.Properties;
using System.Threading;

namespace StarTrekNut.VoiceCom.UI
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Static Fields
        private static readonly SolidColorBrush _brushStatusInfo = new SolidColorBrush(Colors.Green);

        private static readonly SolidColorBrush _brushStatusWarning = new SolidColorBrush(Colors.Blue);

        #endregion

        #region Fields

        private MyDataContext _myDataContext;
        private Timer _timerUpdateCheck;

        #endregion

        #region Constructors and Destructors

        public MainWindow()
        {
            this.InitializeComponent();

            var versionInfo = Assembly.GetExecutingAssembly().GetName().Version;
            this.Title = $"Voice Command Interface v{versionInfo.Major}.{versionInfo.Minor}.{versionInfo.Build}";

            this.Loaded += this.MainWindow_Loaded;
            this.Closing += this.OnClosing;
        }

        #endregion

        #region Properties

        private SpeechProcessor SpeechProcessor { get; set; }

        #endregion

        #region Methods

        private void DisplayApplicationErrorBox(string applicationErrorMessage)
        {
            MessageBox.Show(applicationErrorMessage, "Voice Command Interface: Application Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void DisplayInfoBox(string warningMessage)
        {
            MessageBox.Show(warningMessage, "Voice Command Interface: Info", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void DisplayWarningBox(string warningMessage)
        {
            MessageBox.Show(warningMessage, "Voice Command Interface: Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        private void HookContextEvents(bool attachEvents)
        {
            if (this.SpeechProcessor == null)
                return;

            if (attachEvents)
            {
                this.SpeechProcessor.PropertyChanged += this.SpeechProc_PropertyChanged;
                this.SpeechProcessor.SpeechRecognized += this.SpeechProc_SpeechRecognized;
            }
            else
            {
                this.SpeechProcessor.PropertyChanged -= this.SpeechProc_PropertyChanged;
                this.SpeechProcessor.SpeechRecognized -= this.SpeechProc_SpeechRecognized;
            }
        }

        private VoiceCommandSettings LoadConfig()
        {
            if (Settings.Default.VoiceCommandSettings == null)
            {
                var previousVersion = Properties.Settings.Default.GetPreviousVersion("VoiceCommandSettings");

                if (previousVersion != null)
                {
                    var result = MessageBox.Show(
                        $"User settings weren't found. Would you like to try and import settings from a previous version?",
                        "Voice Command Interface: Question",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Question);

                    if (result == MessageBoxResult.Yes)
                    {
                        // If setting for the current version aren't found, let's look for a previous version.
                        Settings.Default.Upgrade();
                    }
                }

                if (Settings.Default.VoiceCommandSettings == null)
                {
                    Settings.Default.VoiceCommandSettings = new VoiceCommandSettings();
                }
            }
            
            var userConfig = Settings.Default.VoiceCommandSettings.UserList.FirstOrDefault();

            if (userConfig == null)
            {
                Settings.Default.VoiceCommandSettings.UserList.Add(new User());

                MessageBox.Show(
                    $"User settings not found.  Loading default settings.",
                    "Voice Command Interface: Info",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }

            return Settings.Default.VoiceCommandSettings;
        }

        private Executable LoadDefaultExeForUser(User selectedUser)
        {
            if (!selectedUser.ExecutableList.Any())
                return null;
            Executable selectedExe;

            if (string.IsNullOrWhiteSpace(selectedUser.StartupExecutableName))
            {
                selectedExe = selectedUser.ExecutableList.FirstOrDefault();
            }
            else
            {
                selectedExe = selectedUser.ExecutableList.FirstOrDefault(exe => exe.ExecutableName.Equals(selectedUser.StartupExecutableName));
                if (selectedExe == null)
                    selectedExe = selectedUser.ExecutableList.FirstOrDefault();
            }

            return selectedExe;
        }

        private ProfileSettings LoadDefaultProfileForUser(Executable selectedExecutable)
        {
            if (selectedExecutable == null)
                return null;
            if (!selectedExecutable.ProfileSettingsList.Any())
                return null;
            ProfileSettings selectedProfile;

            if (string.IsNullOrWhiteSpace(selectedExecutable.StartupProfileName))
            {
                selectedProfile = selectedExecutable.ProfileSettingsList.FirstOrDefault();
            }
            else
            {
                selectedProfile = selectedExecutable.ProfileSettingsList.FirstOrDefault(prof => prof.ProfileName.Equals(selectedExecutable.StartupProfileName));
                if (selectedProfile == null)
                    selectedProfile = selectedExecutable.ProfileSettingsList.FirstOrDefault();
            }

            return selectedProfile;
        }

        private User LoadUser(VoiceCommandSettings voiceCommandSettings)
        {
            var userSettings = voiceCommandSettings.UserList.FirstOrDefault();
            if (userSettings == null)
            {
                this.DisplayInfoBox("Your windows username was not found in the VoiceComSettings.xml file. The application will load default settings for you, and save them to the file.");
                userSettings = User.GetDefaultForUser();
                voiceCommandSettings.UserList.Add(userSettings);
            }

            return userSettings;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            this.Loaded -= this.MainWindow_Loaded;
            this.Init();
            this.EnableSoftwareUpdateCheck();
        }

        private void EnableSoftwareUpdateCheck()
        {
            if (this._myDataContext.SelectedUser.EnableSoftwareUpdates)
                if(this._timerUpdateCheck == null)
                    this._timerUpdateCheck = new Timer(new TimerCallback(CheckSoftwareVersionCallback), null, 10000, Timeout.Infinite);
        }

        private void DisableSoftwareUpdateCheck()
        {
            if (this._timerUpdateCheck != null)
            {
                this._timerUpdateCheck.Change(Timeout.Infinite, Timeout.Infinite);
                this._timerUpdateCheck.Dispose();
                this._timerUpdateCheck = null;
            }
        }

        private void Init()
        {
            var mainSettings = this.LoadConfig();
            if (mainSettings == null)
                this.Close();

            var user = this.LoadUser(mainSettings);
            var exe = this.LoadDefaultExeForUser(user);
            this.LoadDefaultProfileForUser(exe);

            try
            {
                this.SpeechProcessor = new SpeechProcessor();
            }
            catch (SpeechProcessorNoInputDeviceException)
            {
                this.DisplayApplicationErrorBox("Failed to set default input audio device. Make sure you have a microphone installed and hooked up.");
                this.uiGridMain.IsEnabled = false;
                this.Close();
                return;
            }
            catch (SpeechProcessorNoOutputDeviceException)
            {
                this.DisplayApplicationErrorBox("Failed to set default output audio device. Make sure you have speakers installed and hooked up.");
                this.uiGridMain.IsEnabled = false;
                this.Close();
                return;
            }
            catch (Exception exception)
            {
                this.DisplayApplicationErrorBox($"Failed to initialize the speech processor. Make sure you sound settings and audio devices are hooked up. Reason={exception.Message}");
                this.uiGridMain.IsEnabled = false;
                this.Close();
                return;
            }

            //this.SpeechProcessor.SetHotKeys(user.HotKeysList.Where(hk => hk.GetKey().HasValue).Select(hk => hk.GetKey().Value).ToList());
            this.SpeechProcessor.SetHotKeys(user.HotKeysList.ToList());

            this._myDataContext = new MyDataContext(mainSettings, user, this.SpeechProcessor);

            if (mainSettings.IsNewFile)
                this.uiButtonSaveCharSettings_Click(this, null);

            SendMessageToUi($"Using default microphone: \"{this.SpeechProcessor.DefaultMicrophone}\"");
            this.uiMicrophoneName.Content = this.SpeechProcessor.DefaultMicrophone;

            this.DataContext = this._myDataContext;

            this.HookContextEvents(true);
        }
        
        private void CheckSoftwareVersionCallback(object nothing)
        {
            try
            {
                var webVersion = Lib.Helpers.WebVersionHelper.GetLatestVersionFromUrl();
                if (webVersion == null)
                    throw new Exception("Failed to retrieve version information.");

                var executingAssemblyVersion = Assembly.GetExecutingAssembly().GetName().Version;
                if(webVersion > executingAssemblyVersion)
                {
                    this.Dispatcher.Invoke(() =>
                    {
                        this._myDataContext.HasSoftwareUpdateAvailable = true;
                    });
                }
                else
                {
                    this.Dispatcher.Invoke(() =>
                    {
                        this._myDataContext.HasSoftwareUpdateAvailable = false;
                    });
                }

                this.Dispatcher.Invoke(() =>
                {
                    SendMessageToUi($"Version check completed. v{webVersion.Major}.{webVersion.Minor}.{webVersion.Build} is the latest.");
                });
            }
            catch(System.Exception e)
            {
                this.Dispatcher.Invoke(() =>
                {
                    SendMessageToUi($"Failed to get lastest software version from the web site. Reason=\"{e.Message}\"", Lib.Model.LogEntryType.Warning);
                });
            }
        }
               
        private void OnClosing(object sender, CancelEventArgs cancelEventArgs)
        {
            this.Closing -= this.OnClosing;

            this.DisableSoftwareUpdateCheck();

            this.Cleanup();
        }

        private void Cleanup()
        {
            this.HookContextEvents(false);
            this.SpeechProcessor?.Dispose();
        }
                
        private void SpeechProc_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals("IsRecognizerRunning"))
                SendMessageToUi(this.SpeechProcessor.IsRecognizerRunning ? "Speech recognition is ENABLED" : "Speech recognition is DISABLED");
            else if (e.PropertyName.Equals("IsSelectedProcessRunning"))
                if (this._myDataContext.SpeechProcessor.IsSelectedProcessRunning)
                    this.Dispatcher.Invoke(
                        () =>
                            {
                                this.uiLabelaExeStatus.Foreground = _brushStatusInfo;
                                this.uiLabelaExeStatus.Content = "Running";
                            });
                else
                    this.Dispatcher.Invoke(
                        () =>
                            {
                                this.uiLabelaExeStatus.Foreground = _brushStatusWarning;
                                this.uiLabelaExeStatus.Content = "Not Running";
                            });
        }

        private void SpeechProc_SpeechRecognized(object sender, SpeechProcRecognitionEventArgs e)
        {
            SendMessageToUi(e.RecognitionResultText, e.LogEntryType);
        }

        private void uiButTestTts_Click(object sender, RoutedEventArgs e)
        {
            this.SpeechProcessor.SendTtsTestMessage();
        }

        private void uiButtonAddCharacter_Click(object sender, RoutedEventArgs e)
        {
            #region Do a little validation

            if (string.IsNullOrWhiteSpace(this.uiTextBoxNewCharacterName.Text))
            {
                MessageBox.Show("You need to enter a character profile name.", "Profile Save", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (this._myDataContext.SelectedExecutable.ProfileSettingsList.Any(prof => prof.ProfileName.Equals(this.uiTextBoxNewCharacterName.Text)))
            {
                MessageBox.Show("You need to enter a unique character profile name. The one you choose already exists.", "Profile Save", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            #endregion

            // Create our new character profile
            var profSettings = new ProfileSettings { ProfileName = this.uiTextBoxNewCharacterName.Text, VoiceCommandList = new ObservableCollection<VoiceCommand>() };

            // a dummy/defualt key stroke.
            profSettings.VoiceCommandList.Add(new VoiceCommand
                {
                    Grammer = "foo",
                    KeyStrokes = new System.Collections.Generic.List<System.Windows.Input.Key>() { System.Windows.Input.Key.Space }
                });

            //Optionally copy the currently selected profile settings into the new characters profile
            if (this.uiRadioButtonCopy.IsChecked.HasValue && this.uiRadioButtonCopy.IsChecked.Value)
                if (this._myDataContext.SelectedProfileSettings != null)
                    profSettings.VoiceCommandList = this._myDataContext.SelectedProfileSettings.CloneVoiceCommands();

            if (this.SpeechProcessor != null && this.SpeechProcessor.IsRecognizerRunning)
                this.SpeechProcessor.IsRecognizerRunning = false;

            this._myDataContext.SelectedExecutable.ProfileSettingsList.Add(profSettings);
            this._myDataContext.SelectedProfileSettings = profSettings;
        }
                
        private void uiButtonAddHotkey_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new HotKeyPicker();
            var diagResult = dialog.ShowDialog();

            if (diagResult.HasValue && diagResult.Value)
                this._myDataContext.SelectedUser.HotKeysList.Add(dialog.HotKey);
        }

        private void uiButtonClear_Click(object sender, RoutedEventArgs e)
        {
            if(this._myDataContext != null)
                this._myDataContext.Logger.LogEntries.Clear();
        }

        private void uiButtonRemoveApplication_Click(object sender, RoutedEventArgs e)
        {
            if (sender == this.uiButtonRemoveApplication)
            {
                if (this._myDataContext.SelectedExecutable != null)
                {
                    var confirm = MessageBox.Show(
                    "This will remove your currently selected Application.  Are you sure you want to do this?",
                    "Confirm Removal",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

                    if (confirm == MessageBoxResult.No)
                        return;

                    if (this.SpeechProcessor != null && this.SpeechProcessor.IsRecognizerRunning)
                        this.SpeechProcessor.IsRecognizerRunning = false;

                    this._myDataContext.SelectedUser.ExecutableList.Remove(this._myDataContext.SelectedExecutable);
                }
            }
        }
        
        private void uiButtonRemoveHotKey_Click(object sender, RoutedEventArgs e)
        {
            if (this.uiGridHotKeys.SelectedItem is System.Windows.Input.Key item)
            {
                var confirm = MessageBox.Show(
                    "This will remove your currently selected HotKey.  Are you sure you want to do this?",
                    "Confirm Removal",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

                if (confirm == MessageBoxResult.No)
                    return;

                this._myDataContext.SelectedUser.HotKeysList.Remove(item);
            }
            else
            {
                MessageBox.Show("You haven't selected a HotKey from the grid.", "Command Removal", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void uiButtonRemoveSelectedChar_Click(object sender, RoutedEventArgs e)
        {
            var confirm = MessageBox.Show("This will remove the selected character profile.  Are you sure you want to do this?", "Confirm Removal", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (confirm == MessageBoxResult.No)
                return;

            if (this.SpeechProcessor != null && this.SpeechProcessor.IsRecognizerRunning)
                this.SpeechProcessor.IsRecognizerRunning = false;

            if (this._myDataContext.SelectedProfileSettings != null)
            {
                this._myDataContext.SelectedExecutable.ProfileSettingsList.Remove(this._myDataContext.SelectedProfileSettings);
                this._myDataContext.SelectedProfileSettings = null;
            }
        }

        private void uiButtonSaveCharSettings_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (this.SpeechProcessor != null && this.SpeechProcessor.IsRecognizerRunning)
                    this.SpeechProcessor.IsRecognizerRunning = false;

                Properties.Settings.Default.Save();
                this.SpeechProcessor?.SetUserProfileCommandGrammerKeyStrokes(this._myDataContext.SelectedProfileSettings?.VoiceCommandList?.ToList());
                SendMessageToUi("Profile and settings information has been saved.");
                this._myDataContext.HasProfileChanges = false;
                this._myDataContext.HasSettingsChanges = false;
            }
            catch (Exception exception)
            {
                var message = $"Failed to save profile and settings information to disk.\n\nAdditional Error Info: {exception.Message}";
                SendMessageToUi(message, Lib.Model.LogEntryType.Error);
                this.DisplayApplicationErrorBox(message);
            }
        }

        private void uiButtonAddExe_Click(object sender, RoutedEventArgs e)
        {
            var exePicker = new ExePicker();
            var result = exePicker.ShowDialog();
            if (result.HasValue && result.Value)
            {
                var exeFound = this._myDataContext.SelectedUser.ExecutableList.FirstOrDefault(exe => exe.ExecutableName.Equals(exePicker.SelectedProcessName));
                if (exeFound == null)
                {
                    exeFound = new Executable
                    {
                        ExecutableName = exePicker.SelectedProcessName
                    };

                    this._myDataContext.SelectedUser.ExecutableList.Add(exeFound);
                }

                this._myDataContext.SelectedExecutable = exeFound;
            }
        }

        private void uiComboBoxRecognitionEngines_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.uiComboBoxRecognitionEngines == sender)
                if (this.uiComboBoxRecognitionEngines.SelectedItem is string selectedVoice)
                    this._myDataContext.SelectedUser.StartupSpeechRecogSettings.SelectedEngine = selectedVoice;
        }

        private void uiComboBoxSelectedCharacter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.uiComboBoxSelectedCharacter == sender)
            {
                if (this.uiComboBoxSelectedCharacter.SelectedItem is ProfileSettings selectedProfile)
                {
                    this._myDataContext.SelectedExecutable.StartupProfileName = selectedProfile.ProfileName;
                    this.uiListViewVoiceCommands.IsEnabled = true;
                }
                else
                {
                    this.uiListViewVoiceCommands.IsEnabled = false;
                }
            }
        }

        private void uiComboBoxTtsVoices_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.uiComboBoxTtsVoices == sender)
                if (this.uiComboBoxTtsVoices.SelectedItem is string selectedVoiceString)
                    this._myDataContext.SelectedUser.StartupTtsSettings.SelectedVoice = selectedVoiceString;
        }

        private void uiSliderTtsVolume_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (this.uiSliderTtsVolume == sender)
                this._myDataContext.SelectedUser.StartupTtsSettings.SelectedVolume = (int)this.uiSliderTtsVolume.Value;
        }
                
        private void UiButtonAddVoiceCommand_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button == null) return;

            if (button.Tag.Equals("AddVoiceCommand"))
            {
                var dialog = new VoiceCommandEditor(
                    EditorType.Add, 
                    this._myDataContext.SelectedProfileSettings.VoiceCommandList);

                dialog.VoiceCommand = new VoiceCommand();
                dialog.Owner = this;

                var diagResult = dialog.ShowDialog();

                if (diagResult.HasValue && diagResult.Value)
                {
                    this._myDataContext.SelectedProfileSettings.VoiceCommandList.Add(dialog.VoiceCommand);
                }
            }
        }

        private void ButtonEditVoiceCommand_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button == null) return;

            if (button.Tag.Equals("EditVoiceCommand"))
            {
                var voiceCommand = button.CommandParameter as VoiceCommand;
                if (voiceCommand == null) return;

                var dialog = new VoiceCommandEditor(
                    EditorType.Edit, 
                    this._myDataContext.SelectedProfileSettings.VoiceCommandList);

                dialog.VoiceCommand = voiceCommand;
                dialog.Owner = this;

                var diagResult = dialog.ShowDialog();
            }
        }

        private void ButtonCopyVoiceCommand_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button == null) return;

            if (button.Tag.Equals("CopyVoiceCommand"))
            {
                var voiceCommand = button.CommandParameter as VoiceCommand;
                if (voiceCommand == null) return;

                var dialog = new VoiceCommandEditor(
                    EditorType.Copy, 
                    this._myDataContext.SelectedProfileSettings.VoiceCommandList);

                dialog.VoiceCommand = new VoiceCommand()
                {
                    Grammer = $"Copy Of {voiceCommand.Grammer}",
                    KeyStrokes = voiceCommand.KeyStrokes
                };
                dialog.Owner = this;

                var diagResult = dialog.ShowDialog();

                if (diagResult.HasValue && diagResult.Value)
                {
                    this._myDataContext.SelectedProfileSettings.VoiceCommandList.Add(dialog.VoiceCommand);
                }
            }
        }

        private void ButtonDeleteVoiceCommand_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button == null) return;

            if (button.Tag.Equals("DeleteVoiceCommand"))
            {
                var voiceCommand = button.CommandParameter as VoiceCommand;
                if (voiceCommand != null)
                {
                    var confirm = MessageBox.Show(
                        "This will remove your Voice Command.  Are you sure you want to do this?",
                        "Confirm Removal",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Warning);

                    if (confirm == MessageBoxResult.No)
                        return;

                    this._myDataContext.SelectedProfileSettings.VoiceCommandList.Remove(voiceCommand);
                }
                else
                {
                    MessageBox.Show("You haven't selected a Voice Command from the grid.", "Command Removal", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void EnableProfileSection(bool enable)
        {
            label1.IsEnabled = enable;
            uiComboBoxSelectedCharacter.IsEnabled = enable;
            uiButtonRemoveSelectedChar.IsEnabled = enable;
            uiGroupBoxCreateProfile.IsEnabled = enable;
        }
               
        private void UiComboBoxApplication_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this._myDataContext.SelectedExecutable == null)
            {
                this.EnableProfileSection(false);
            }
            else
            {
                this.EnableProfileSection(true);
            }
        }

        private void UiButtonImport_Click(object sender, RoutedEventArgs e)
        {
            var confirm = MessageBox.Show(
                        "This will overwrite your existing settings.  Are you sure you want to do this?",
                        "Confirm Import",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Warning);

            if (confirm == MessageBoxResult.No)
                return;

            var dialog = new System.Windows.Forms.OpenFileDialog();
            dialog.Filter = "XML files (*.xml)|*.xml|All files (*.*)|*.*";
            dialog.Multiselect = false;
            dialog.CheckFileExists = true;

            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                VoiceCommandSettings tmpVoiceCommandSettings = null;

                try
                {
                    tmpVoiceCommandSettings = StarTrekNut.VoiceCom.Lib.Helpers.VoiceCommandSettingsHelper.ReadFile(dialog.FileName);
                    if (tmpVoiceCommandSettings == null)
                        throw new Exception("Failed to read file.");
                }
                catch (Exception exception)
                {
                    MessageBox.Show(
                        $"The import failed.\n\nReason \"{exception.Message}\"",
                        "Confirm Import",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
                }

                if (tmpVoiceCommandSettings != null)
                {
                    try
                    {
                        this.Cleanup();
                        Settings.Default.VoiceCommandSettings = tmpVoiceCommandSettings;
                        this.Init();
                    }
                    catch (System.Exception exception)
                    {
                        MessageBox.Show(
                        $"The import failed to re-initialize the application.\n\nReason \"{exception.Message}\"\n\nClosing the application.",
                        "Confirm Import",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);

                        this.Close();
                    }
                }
            }

            dialog.Dispose();
        }

        private void UiButtonExport_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new System.Windows.Forms.SaveFileDialog();
            dialog.Filter = "XML files (*.xml)|*.xml|All files (*.*)|*.*";

            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                try
                {
                    Lib.Helpers.VoiceCommandSettingsHelper.WriteFile(dialog.FileName, this._myDataContext.VoiceComSettings);

                    MessageBox.Show(
                        $"The export is complete.",
                        "Confirm Export",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);
                }
                catch (System.Exception exception)
                {
                    MessageBox.Show(
                        $"The export failed.\n\nReason \"{exception.Message}\"",
                        "Confirm Export",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
                }
            }

            dialog.Dispose();
        }

        private void UiButtonHelp_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var processHandle = System.Diagnostics.Process.Start(Lib.Helpers.WebVersionHelper._projectUrl);
                processHandle.Dispose();
                processHandle = null;
            }
            catch(Exception exception)
            {
                DisplayApplicationErrorBox("Failed to open application web page. Confirm you have a default web browser selected in Windows.");
                SendMessageToUi($"Failed to open application web page. Error={exception.Message}", Lib.Model.LogEntryType.Error);
                return;
            }
        }
        
        private void SendMessageToUi(string message, Lib.Model.LogEntryType logEntryType = Lib.Model.LogEntryType.Info)
        {
            if (this._myDataContext == null)
                return;

            if (logEntryType == Lib.Model.LogEntryType.Error)
            {
                this._myDataContext.Logger.Error(message);
            }
            else if (logEntryType == Lib.Model.LogEntryType.Warning)
            {
                this._myDataContext.Logger.Warning(message);
            }
            else
            {
                this._myDataContext.Logger.Info(message);
            }
        }
        #endregion
    }
}