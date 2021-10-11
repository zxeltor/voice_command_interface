using System;
using System.Collections.Specialized;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

using StarTrekNut.VoiceCom.Lib;
using StarTrekNut.VoiceCom.Lib.Model.VoiceComSettings;

namespace StarTrekNut.VoiceCom.UI.Classes
{
    public class MyDataContext : INotifyPropertyChanged, IDisposable
    {
        #region Fields

        private bool _hasProfileChanges;
        private bool _hasSoftwareUpdateAvailable;
        private bool _hasSettingsChanges;

        private ClientMessageLogger _logger;

        private Executable _selectedExecutable;

        private ProfileSettings _selectedProfileSettings;

        private User _selectedUser;

        private SpeechProcessor _speechProcessor;

        private VoiceCommandSettings _voiceComSettings;

        #endregion

        #region Constructors and Destructors

        public MyDataContext(VoiceCommandSettings mainSettings, User selectedUser, SpeechProcessor speechProcessor)
        {
            this.SpeechProcessor = speechProcessor;
            this.Logger = ClientMessageLogger.Instance;

            this.PropertyChanged += this.MyDataContextPropertyChanged;

            this.VoiceComSettings = mainSettings;
            this.SelectedUser = selectedUser;

            this.AttachUserPropertyEvents();

            if (this.SelectedUser?.StartupTtsSettings != null)
            {
                this.SelectedUser.StartupTtsSettings.EnableTtsCommandAck = this.SelectedUser.StartupTtsSettings.EnableTtsCommandAck;
            }
        }

        ~MyDataContext()
        {
            this.Dispose();
        }

        #endregion

        #region Public Events

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Public Properties
        public bool HasProfileChanges
        {
            get => this._hasProfileChanges;
            set
            {
                this._hasProfileChanges = value;
                this.NotifyPropertyChange("HasProfileChanges");
                this.NotifyPropertyChange("HasChanges");
            }
        }

        public bool HasSoftwareUpdateAvailable
        {
            get => this._hasSoftwareUpdateAvailable;
            set
            {
                this._hasSoftwareUpdateAvailable = value;
                this.NotifyPropertyChange("HasSoftwareUpdateAvailable");
            }
        }

        public bool HasSettingsChanges
        {
            get => this._hasSettingsChanges;
            set
            {
                this._hasSettingsChanges = value;
                this.NotifyPropertyChange("HasSettingsChanges");
                this.NotifyPropertyChange("HasChanges");
            }
        }

        public bool HasChanges
        {
            get => this._hasSettingsChanges || this._hasProfileChanges;
        }

        public ClientMessageLogger Logger
        {
            get => this._logger;
            set
            {
                this._logger = value;
                this.NotifyPropertyChange("Logger");
            }
        }

        public Executable SelectedExecutable
        {
            get => this._selectedExecutable;
            set
            {
                this._selectedExecutable = value;
                this.NotifyPropertyChange("SelectedExecutable");
            }
        }

        public ProfileSettings SelectedProfileSettings
        {
            get => this._selectedProfileSettings;
            set
            {
                this._selectedProfileSettings = value;
                this.NotifyPropertyChange("SelectedProfileSettings");
            }
        }

        public User SelectedUser
        {
            get => this._selectedUser;
            set
            {
                this._selectedUser = value;
                this.NotifyPropertyChange("SelectedUser");
            }
        }

        public SpeechProcessor SpeechProcessor
        {
            get => this._speechProcessor;
            set
            {
                this._speechProcessor = value;
                this.NotifyPropertyChange("SpeechProcessor");
            }
        }

        public VoiceCommandSettings VoiceComSettings
        {
            get => this._voiceComSettings;
            set
            {
                this._voiceComSettings = value;
                this.NotifyPropertyChange("VoiceComSettings");
            }
        }

        #endregion

        #region Public Methods and Operators

        public void AttachUserPropertyEvents()
        {
            this.VoiceComSettings.PropertyChanged += this.VoiceComSettingsPropertyChanged;
            this.VoiceComSettings.UserList.ForEach(
                user =>
                    {
                        user.PropertyChanged += this.UserPropertyChanged;
                        user.StartupTtsSettings.PropertyChanged += this.StartupTtsSettingsPropertyChanged;
                        user.StartupSpeechRecogSettings.PropertyChanged += this.StartupSpeechRecogSettings_PropertyChanged;
                        user.HotKeysList.CollectionChanged += this.HotKeysListCollectionChanged;   //.ToList().ForEach(hotkey => hotkey.PropertyChanged += this.Hotkey_PropertyChanged);
                        user.ExecutableList.ToList().ForEach(
                            exe =>
                                {
                                    exe.PropertyChanged += this.ExePropertyChanged;
                                    exe.ProfileSettingsList.ToList().ForEach(
                                        prof =>
                                            {
                                                prof.PropertyChanged += this.Prof_PropertyChanged;
                                                prof.VoiceCommandList.CollectionChanged += this.VoiceCommandListCollectionChanged;
                                                prof.VoiceCommandList.ToList().ForEach(com => com.PropertyChanged += this.VoiceCommandPropertyChanged);
                                            });
                                    exe.KeyTranslations.CollectionChanged += KeyTranslations_CollectionChanged;
                                    exe.KeyTranslations.ToList().ForEach(tran => tran.PropertyChanged += Tran_PropertyChanged);
                                });
                    });
        }

        private void Tran_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            this.HasProfileChanges = true;
        }

        private void KeyTranslations_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            var newItems = e.NewItems;
            if (newItems != null)
                foreach (var keyTranslation in newItems)
                {
                    ((KeyTranslation)keyTranslation).PropertyChanged += this.Tran_PropertyChanged;
                }

            var oldItems = e.OldItems;
            if (oldItems != null)
                foreach (var keyTranslation in oldItems)
                {
                    ((KeyTranslation)keyTranslation).PropertyChanged -= this.Tran_PropertyChanged;
                }

            this.HasProfileChanges = true;
        }

        private void HotKeysListCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if(sender is ObservableCollection<HotKey> collection)
            {
                this.SpeechProcessor?.SetHotKeys(collection.Where(hk => hk.GetKey().HasValue).Select(hk => hk.GetKey().Value).ToList());
            }
        }

        public void DetachUserPropertyEvents()
        {
            this.VoiceComSettings.PropertyChanged -= this.VoiceComSettingsPropertyChanged;
            this.VoiceComSettings.UserList.ForEach(
                user =>
                    {
                        user.PropertyChanged -= this.UserPropertyChanged;
                        user.StartupTtsSettings.PropertyChanged -= this.StartupTtsSettingsPropertyChanged;
                        user.StartupSpeechRecogSettings.PropertyChanged -= this.StartupSpeechRecogSettings_PropertyChanged;
                        user.HotKeysList.CollectionChanged -= this.HotKeysListCollectionChanged;   //.ToList().ForEach(hotkey => hotkey.PropertyChanged -= this.Hotkey_PropertyChanged);
                        user.ExecutableList.ToList().ForEach(
                            exe =>
                                {
                                    exe.PropertyChanged -= this.ExePropertyChanged;
                                    exe.ProfileSettingsList.ToList().ForEach(
                                        prof =>
                                            {
                                                prof.PropertyChanged -= this.Prof_PropertyChanged;
                                                prof.VoiceCommandList.CollectionChanged -= this.VoiceCommandListCollectionChanged;
                                                prof.VoiceCommandList.ToList().ForEach(com => com.PropertyChanged -= this.VoiceCommandPropertyChanged);
                                            });
                                    exe.KeyTranslations.CollectionChanged -= KeyTranslations_CollectionChanged;
                                    exe.KeyTranslations.ToList().ForEach(tran => tran.PropertyChanged -= Tran_PropertyChanged);
                                });
                    });
        }

        public void Dispose()
        {
            this.DetachUserPropertyEvents();
        }

        #endregion

        #region Methods

        private void ExePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "StartupProfileName":
                    this.Logger.Info($"Profile was changed to: \"{this.SelectedProfileSettings.ProfileName}\"");
                    break;
            }
        }
        
        private void MyDataContextPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals("SelectedUser"))
            {
                var defExeName = this.SelectedUser?.StartupExecutableName;
                if(!string.IsNullOrWhiteSpace(defExeName))
                {
                    var defExe = this.SelectedUser.ExecutableList.FirstOrDefault(exe => exe.ExecutableName.Equals(defExeName));
                    if (defExe != null)
                    {
                        this.SelectedExecutable = defExe;
                    }
                }
            }
            else if (e.PropertyName.Equals("SelectedExecutable"))
            {
                this.SpeechProcessor.RunningProcessName = this.SelectedExecutable?.ExecutableName;
                this.SelectedUser.StartupExecutableName = this.SelectedExecutable?.ExecutableName;

                var startupProfName = this.SelectedExecutable?.StartupProfileName;
                if (!string.IsNullOrWhiteSpace(startupProfName))
                {
                    var defaultProf = this.SelectedExecutable?.ProfileSettingsList.FirstOrDefault(prof => prof.ProfileName.Equals(startupProfName));
                    if (defaultProf != null)
                    {
                        this.SelectedProfileSettings = defaultProf;
                    }
                }
            }
            else if (e.PropertyName.Equals("SelectedProfileSettings"))
            {
                if (this.SelectedProfileSettings != null)
                {
                    this.SelectedProfileSettings.VoiceCommandList.CollectionChanged -= this.VoiceCommandListCollectionChanged;
                    this.SelectedProfileSettings.VoiceCommandList.CollectionChanged += this.VoiceCommandListCollectionChanged;
                }

                this.SpeechProcessor.SetUserProfileCommandGrammerKeyStrokes(this.SelectedProfileSettings?.VoiceCommandList?.ToList());
            }
            else if(e.PropertyName.Equals("HasProfileChanges"))
            {
                if(this.HasProfileChanges)
                {
                    if(this.SpeechProcessor.IsRecognizerRunning)
                    {
                        this.SpeechProcessor.IsRecognizerRunning = false;
                    }
                }
            }
        }

        private void NotifyPropertyChange(string propertyName)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void Prof_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
        }

        private void StartupSpeechRecogSettings_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
        }

        private void StartupTtsSettingsPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "SelectedVoice":
                    var voiceFound = this.SpeechProcessor.TtsVoices.FirstOrDefault(voice => voice.VoiceInfo.Name.Equals(this.SelectedUser.StartupTtsSettings.SelectedVoice));
                    if (voiceFound != null)
                    {
                        this.SpeechProcessor.SelectedTtsVoice = voiceFound;
                        this.HasSettingsChanges = true;
                    }
                    break;
                case "SelectedVolume":
                    this.SpeechProcessor.TtsVolume = this.SelectedUser.StartupTtsSettings.SelectedVolume;
                    this.HasSettingsChanges = true;
                    break;
                case "EnableTtsCommandAck":
                    this.SpeechProcessor.EnableCommandAck = this.SelectedUser.StartupTtsSettings.EnableTtsCommandAck;
                    this.HasSettingsChanges = true;
                    break;
            }
        }

        private void UserPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "StartupExecutableName":
                    this.SpeechProcessor.RunningProcessName = this.SelectedUser.StartupExecutableName;
                    break;
            }
        }

        private void VoiceCommandPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            this.HasProfileChanges = true;
        }

        private void VoiceCommandListCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            var newItems = e.NewItems;
            if (newItems != null)
                foreach (var voiceCommand in newItems)
                {
                    ((VoiceCommand)voiceCommand).PropertyChanged += this.VoiceCommandPropertyChanged;
                }

            var oldItems = e.OldItems; // as System.Collections.re    //List<VoiceCommand>;
            if (oldItems != null)
                foreach (var voiceCommand in oldItems)
                {
                    ((VoiceCommand)voiceCommand).PropertyChanged -= this.VoiceCommandPropertyChanged;
                }

            this.HasProfileChanges = true;
        }

        private void VoiceComSettingsPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
        }

        #endregion
    }
}