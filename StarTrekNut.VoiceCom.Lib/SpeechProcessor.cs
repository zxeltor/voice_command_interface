using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Speech.Recognition;
using System.Speech.Synthesis;
using System.Threading;
using System.Windows.Input;

using StarTrekNut.Utils;
using StarTrekNut.VoiceCom.Lib.Classes;
using StarTrekNut.VoiceCom.Lib.Model.VoiceComSettings;

namespace StarTrekNut.VoiceCom.Lib
{
    /// <summary>
    ///     The class used to manage speech recognition and text-to-speech for this application. This is a singleton class.
    /// </summary>
    public class SpeechProcessor : INotifyPropertyChanged, IDisposable
    {
        #region Static Fields

        //private static SpeechProcessor _instance;

        /// <summary>
        ///     The voice command used to enable voice commands
        /// </summary>
        private static readonly VoiceCommand _commandResume = new VoiceCommand { Grammer = "enable voice commands", KeyStrokes = "enabling voice commands." };

        private static readonly VoiceCommand _commandDisable = new VoiceCommand { Enabled = true, Grammer = "disable voice commands", KeyStrokes = "disable voice commands." };

        #endregion

        #region Fields

        /// <summary>
        ///     A collection of application level voice commands
        /// </summary>
        private readonly Grammar _applicationCommandGrammer = new Grammar(new GrammarBuilder(new Choices(_commandResume.Grammer)));

        /// <summary>
        ///     List of Hotkeys which temporarily disable voice recognition while they're pressed.
        /// </summary>
        private List<Key> _hotKeys;

        /// <summary>
        ///     Used to determine if the World of Warcraft executable is running.
        /// </summary>
        private bool _isSelecetdProcessRunning;

        /// <summary>
        ///     Used to disable/stop the thread _threadCheckForHotKeyPressed
        /// </summary>
        private volatile bool _isThreadCheckForHotKeyPressedRunning;

        //private VoiceCommandSettings _voiceCommandSettings;
        private List<VoiceCommand> _profileCommands;

        /// <summary>
        ///     A reference to the Speech recognition engine object
        /// </summary>
        private SpeechRecognitionEngine _recognizer;

        /// <summary>
        ///     The currently selected speech recognizer from the Windows Operating System
        /// </summary>
        private RecognizerInfo _selectedRecognizer;

        /// <summary>
        ///     The currently selected Text-to-Speech voice from the Windows Operating System
        /// </summary>
        private InstalledVoice _selectedTtsVoice;

        /// <summary>
        ///     Boolean used to determine if the speech recognition engine is paused do to a Hotkey being pressed.
        /// </summary>
        private volatile bool _speachRecogIsPaused;

        /// <summary>
        ///     A handle to the thread used to monitor Hotkeys being pressed.
        /// </summary>
        private Thread _threadCheckForHotKeyPressed;

        /// <summary>
        ///     A timer used to check for the existence of the World of Warcraft process
        /// </summary>
        private Timer _timerCheckIfExeIsRunning;

        /// <summary>
        ///     A reference to the Text-to-Speech object
        /// </summary>
        private SpeechSynthesizer _ttsEngine;

        /// <summary>
        ///     A list of available Text-to-Speech voice from the Windows Operating System
        /// </summary>
        private List<InstalledVoice> _ttsVoices;

        /// <summary>
        ///     The currently set output volume for Text-to-Speech
        /// </summary>
        private int _ttsVolume = 20;

        public string DefaultMicrophone { get; private set; }

        #endregion

        #region Constructors and Destructors

        public SpeechProcessor()
        {
            this._timerCheckIfExeIsRunning = new Timer(this.CheckIfProcessIsRunningCallback, null, 4000, 10000);

            //RecogEngines = SpeechRecognitionEngine.InstalledRecognizers().ToList();
            this._selectedRecognizer = RecogEngines.FirstOrDefault();
            //_profileCommands = profileCommands;

            this.StartTts();
            this.Start();
        }

        #endregion

        #region Public Events

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        ///     Event handler used to pass along messages to the user Window.
        /// </summary>
        public event EventHandler<SpeechProcRecognitionEventArgs> SpeechRecognized;

        #endregion

        #region Public Properties

        /// <summary>
        ///     Get and Private Set. A collection of supported speech recognizers from the Windows Operating System.
        /// </summary>
        public static List<RecognizerInfo> RecogEngines => SpeechRecognitionEngine.InstalledRecognizers().ToList();

        /// <summary>
        ///     Get the singleton instance of this class
        /// </summary>
        /// <summary>
        ///     Get and Set a list of character/profile voice commands
        /// </summary>
        /// <summary>
        ///     Get and Set. Boolean used to determine if the application should send Text-to-Speech verification messages
        ///     to the user when a voice command is detected or other application level event occurs.
        /// </summary>
        public bool EnableCommandAck
        {
            get => this._ttsEngine != null;
            set
            {
                if (value)
                    this.StartTts();
                else
                    this.StopTts();
                this.NotifyPropertyChange("EnableCommandAck");
            }
        }

        /// <summary>
        ///     Get and Set. Used to determine if the Speech recognition engine is running.
        /// </summary>
        public bool IsRecognizerRunning
        {
            get => this._recognizer != null;
            set
            {
                if (value)
                {
                    this.SendTtsAcknowledge("Starting speech recognition");
                    this.Start();
                }
                else
                {
                    this.SendTtsAcknowledge("Stopping speech recognition");
                    this.Stop();
                }

                this.NotifyPropertyChange("IsRecognizerRunning");
            }
        }

        /// <summary>
        ///     Get and Private Set. Used to determine if the World of Warcraft executable is running.
        /// </summary>
        public bool IsSelectedProcessRunning
        {
            get => this._isSelecetdProcessRunning;
            private set
            {
                if (this._isSelecetdProcessRunning == value)
                    return;
                this._isSelecetdProcessRunning = value;
                this.NotifyPropertyChange("IsSelectedProcessRunning");
            }
        }

        /// <summary>
        ///     Get and Set the name of the World of Warcraft executable.
        /// </summary>
        public string RunningProcessName { get; set; }

        /// <summary>
        ///     Get and Set. The currently selected speech recognizer from the Windows Operating System
        /// </summary>
        public RecognizerInfo SelectedRecognizer
        {
            get => this._selectedRecognizer;
            set
            {
                if (this._selectedRecognizer == value)
                    return;
                this._selectedRecognizer = value;
                this.NotifyPropertyChange("SelectedRecognizer");
                this.StopAndRestartRecog();
            }
        }

        /// <summary>
        ///     Get and Set. The currently selected Text-to-Speech voice from the Windows Operating System
        /// </summary>
        public InstalledVoice SelectedTtsVoice
        {
            get => this._selectedTtsVoice;
            set
            {
                if (this._selectedTtsVoice.Equals(value))
                    return;
                this._selectedTtsVoice = value;
                this.ResetTtsVoice();
                this.SendTtsTestMessage();
                this.NotifyPropertyChange("SelectedTtsVoice");
            }
        }

        /// <summary>
        ///     Get and Set. Boolean used to determine if the speech recognition engine is paused do to a Hotkey being pressed.
        /// </summary>
        public bool SpeachRecogIsPaused => this._speachRecogIsPaused;

        /// <summary>
        ///     Get and Private Set. A list of available Text-to-Speech voice from the Windows Operating System
        /// </summary>
        public List<InstalledVoice> TtsVoices
        {
            get => this._ttsVoices;
            private set
            {
                this._ttsVoices = value;
                this.NotifyPropertyChange("TtsVoices");
            }
        }

        /// <summary>
        ///     Get and Set. The currently selected output volume for Text-to-Speech. 0-100 (Represents percentage).
        /// </summary>
        public int TtsVolume
        {
            get => this._ttsVolume;
            set
            {
                this._ttsVolume = value;
                this.NotifyPropertyChange("TtsVolume");
            }
        }

        #endregion

        #region Properties

        /// <summary>
        ///     A handle to the World of Warcraft process.
        /// </summary>
        private Process RunningProcess { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Dispose of this object. Hopefully properlly.
        /// </summary>
        public void Dispose()
        {
            this.Stop();
            this.StopTts();

            if (this._timerCheckIfExeIsRunning != null)
            {
                this._timerCheckIfExeIsRunning.Change(Timeout.Infinite, Timeout.Infinite);
                this._timerCheckIfExeIsRunning.Dispose();
                this._timerCheckIfExeIsRunning = null;
            }
        }

        /// <summary>
        ///     Send an async ack to the Text-to-Speech engine. Used when a user selected a new Text-to-Speech voice in the main
        ///     Window.
        /// </summary>
        public void SendTtsTestMessage()
        {
            if (this._ttsEngine == null)
                return;
            this._ttsEngine.Volume = this.TtsVolume;
            this._ttsEngine.SpeakAsync("Welcome from the voice command interface.");
        }

        /// <summary>
        ///     Set the default Text-to-Speech voice.
        /// </summary>
        /// <param name="defVoice">The name of the default voice</param>
        public void SetDefaultTtsVoice(string defVoice)
        {
            if (!string.IsNullOrWhiteSpace(defVoice))
            {
                var defaultTtsVoice = this.TtsVoices.FirstOrDefault(voice => voice.VoiceInfo.Name.ToLower().Contains(defVoice.ToLower()));
                if (defaultTtsVoice != null)
                {
                    this._selectedTtsVoice = defaultTtsVoice;
                    this.ResetTtsVoice();
                    this.NotifyPropertyChange("SelectedTtsVoice");
                }
            }
        }

        /// <summary>
        ///     Used to set the voice command interrupt Hotkeys
        /// </summary>
        /// <param name="hotKeys">List of keys</param>
        public void SetHotKeys(List<Key> hotKeys)
        {
            if (this._threadCheckForHotKeyPressed != null)
            {
                this._isThreadCheckForHotKeyPressedRunning = false;
                this._threadCheckForHotKeyPressed = null;
            }

            if (hotKeys != null)
            {
                this._hotKeys = hotKeys;

                this._threadCheckForHotKeyPressed = new Thread(this.CheckIfHotKeyIsPressedCallback);
                this._threadCheckForHotKeyPressed.SetApartmentState(ApartmentState.STA);
                this._threadCheckForHotKeyPressed.Start();
            }
        }

        /// <summary>
        ///     Send the profile voice commands to the Speech Recognition Engine
        /// </summary>
        /// <param name="profileCommands">List of voice commands</param>
        public void SetUserProfileCommandGrammerKeyStrokes(List<VoiceCommand> profileCommands)
        {
            if (profileCommands == null)
                this._profileCommands = new List<VoiceCommand>();
            else
                this._profileCommands = profileCommands;

            if (this._recognizer == null)
                return;

            if (this._recognizer.AudioState == AudioState.Stopped && this._profileCommands.Any())
            {
                if (!this._recognizer.Grammars.Any())
                    this._recognizer.LoadGrammar(this._applicationCommandGrammer);
                this._recognizer.RecognizeAsync(RecognizeMode.Multiple);
            }

            this._recognizer.RequestRecognizerUpdate(this._profileCommands);
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Callback handler for the _threadCheckForHotKeyPressed thread.
        /// </summary>
        private void CheckIfHotKeyIsPressedCallback()
        {
            this._isThreadCheckForHotKeyPressedRunning = true;

            while (this._isThreadCheckForHotKeyPressedRunning)
            {
                if (this._hotKeys != null && this._hotKeys.Any())
                {
                    var keyFound = this._hotKeys.Any(hotkey => Keyboard.IsKeyDown(hotkey));

                    if (keyFound)
                    {
                        if (!this._speachRecogIsPaused)
                        {
                            this._recognizer.RecognizeAsyncCancel();
                            this._speachRecogIsPaused = true;
                            this.NotifyPropertyChange("SpeachRecogIsPaused");
                        }
                    }
                    else
                    {
                        if (this._speachRecogIsPaused)
                        {
                            this._recognizer.RecognizeAsync(RecognizeMode.Multiple);
                            this._speachRecogIsPaused = false;
                            this.NotifyPropertyChange("SpeachRecogIsPaused");
                        }
                    }
                }
                else
                {
                    if (this._speachRecogIsPaused)
                    {
                        this._recognizer.RecognizeAsync(RecognizeMode.Multiple);
                        this._speachRecogIsPaused = false;
                        this.NotifyPropertyChange("SpeachRecogIsPaused");
                    }
                }

                try
                {
                    if (this._isThreadCheckForHotKeyPressedRunning)
                        Thread.Sleep(100);
                }
                catch (ThreadInterruptedException)
                {
                    this._isThreadCheckForHotKeyPressedRunning = false;
                }
                catch (ThreadAbortException)
                {
                    this._isThreadCheckForHotKeyPressedRunning = false;
                }
            }
        }

        /// <summary>
        ///     Callback handler for the _timerCheckIfWowIsRunning timer.
        /// </summary>
        private void CheckIfProcessIsRunningCallback(object something)
        {
            this._timerCheckIfExeIsRunning.Change(Timeout.Infinite, Timeout.Infinite);
            if (!string.IsNullOrWhiteSpace(this.RunningProcessName))
                this.IsSelectedProcessRunning = ProcessHelpers.IsProcessRunning(this.RunningProcessName);
            this._timerCheckIfExeIsRunning.Change(10000, 10000);
        }

        /// <summary>
        ///     Enable or Disable events handled by the Speech Recognition Engine.
        /// </summary>
        /// <param name="hookEvents">True to enable, False otherwise.</param>
        private void HookupRecognizerEvents(bool hookEvents)
        {
            if (hookEvents)
            {
                this._recognizer.SpeechRecognized += this.RecognizerOnSpeechRecognized;
                this._recognizer.RecognizerUpdateReached += this.RecognizerOnRecognizerUpdateReached;
            }
            else
            {
                this._recognizer.SpeechRecognized -= this.RecognizerOnSpeechRecognized;
                this._recognizer.RecognizerUpdateReached -= this.RecognizerOnRecognizerUpdateReached;
            }
        }

        private void NotifyPropertyChange(string propertyName)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        ///     Event handler for the Speech Recognition Engine. This is fired when an update state is reached.
        /// </summary>
        /// <param name="sender">The sender</param>
        /// <param name="recognizerUpdateReachedEventArgs">Event args</param>
        private void RecognizerOnRecognizerUpdateReached(object sender, RecognizerUpdateReachedEventArgs recognizerUpdateReachedEventArgs)
        {
            if (recognizerUpdateReachedEventArgs.UserToken is List<VoiceCommand>)
            {
                if (this._recognizer == null)
                    return;

                this._recognizer.UnloadAllGrammars();

                if (!this._profileCommands.Any())
                    return;

                if (!this._profileCommands.Any(com => com.Enabled))
                    return;

                this._profileCommands.Add(_commandDisable);

                var choices = new Choices();
                
                this._profileCommands.ForEach(
                    gramKey =>
                        {
                            if (gramKey.Enabled && !string.IsNullOrWhiteSpace(gramKey.Grammer))
                                choices.Add(gramKey.Grammer);
                        });

                this._recognizer.LoadGrammar(new Grammar(new GrammarBuilder(choices)));
            }
        }

        /// <summary>
        ///     Event handler for the Speech Recognition Engine. This is fired when a configured voice command was detected.
        /// </summary>
        /// <param name="sender">The sender</param>
        /// <param name="e">Event args</param>
        private void RecognizerOnSpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            var gramerKeystroke = this._profileCommands?.FirstOrDefault(key => key.Grammer.Equals(e.Result.Text));
            if (gramerKeystroke == null)
                return;
            if (string.IsNullOrWhiteSpace(gramerKeystroke.KeyStrokes))
                return;

            if (this.RunningProcess == null)
            {
                this.RunningProcess = ProcessHelpers.FindRunningProcess(this.RunningProcessName);
            }
            else if (this.RunningProcess.HasExited)
            {
                this.RunningProcess.Dispose();
                this.RunningProcess = ProcessHelpers.FindRunningProcess(this.RunningProcessName);
            }

            if (gramerKeystroke.Grammer.Equals(_commandDisable.Grammer))
            {
                this.IsRecognizerRunning = false;
            }
            else if (this.RunningProcess == null)
            {
                this.SendTtsAcknowledge("Command received but application is not running");
            }
            else
            {
                ProcessHelpers.SendKeysToApplication(gramerKeystroke.KeyStrokes, this.RunningProcess);
                this.SendTtsAcknowledge(e.Result.Text);
            }

            this.SendCommandToUserInterfaceLog(sender, new SpeechProcRecognitionEventArgs(e.Result.Text));
        }

        /// <summary>
        ///     Set the voice for the Text-to-Speech engine
        /// </summary>
        private void ResetTtsVoice()
        {
            this._ttsEngine?.SelectVoice(this.SelectedTtsVoice.VoiceInfo.Name);
        }

        /// <summary>
        ///     Used to send messages to the user Window
        /// </summary>
        /// <param name="sender">The sender</param>
        /// <param name="e">Events args</param>
        private void SendCommandToUserInterfaceLog(object sender, SpeechProcRecognitionEventArgs e)
        {
            this.SpeechRecognized?.Invoke(sender, e);
        }

        /// <summary>
        ///     Send an async ack to the Text-to-Speech engine.
        /// </summary>
        /// <param name="message">The text to send</param>
        private void SendTtsAcknowledge(string message)
        {
            if (string.IsNullOrWhiteSpace(message) || this._ttsEngine == null)
                return;
            this._ttsEngine.Volume = this.TtsVolume;
            this._ttsEngine.SpeakAsync(message);
        }

        /// <summary>
        ///     Start the Speech Recognition Engine
        /// </summary>
        private void Start()
        {
            this._recognizer = this.SelectedRecognizer == null ? new SpeechRecognitionEngine(new CultureInfo("en-US")) : new SpeechRecognitionEngine(this.SelectedRecognizer);

            try
            {
                this._recognizer.SetInputToDefaultAudioDevice();
            }
            catch (Exception e)
            {
                throw new SpeechProcessorNoInputDeviceException("Failed to set input to default audio device.", e);
            }

            try
            {
                using (var deviceEnum = new NAudio.CoreAudioApi.MMDeviceEnumerator())
                {
                    var defaultInputDevice = deviceEnum.GetDefaultAudioEndpoint(NAudio.CoreAudioApi.DataFlow.Capture, NAudio.CoreAudioApi.Role.Console);

                    if(defaultInputDevice != null)
                    {
                        this.DefaultMicrophone = defaultInputDevice.DeviceFriendlyName;
                    }
                }
            }
            catch(Exception e)
            {
                throw new SpeechProcessorNoInputDeviceException("Failed to get default audio input device name.", e);
            }
            
            this.HookupRecognizerEvents(true);

            if (this._hotKeys != null && this._hotKeys.Any())
                this.SetHotKeys(this._hotKeys.ToList());

            if (this._profileCommands != null)
                this.SetUserProfileCommandGrammerKeyStrokes(this._profileCommands);

            this.NotifyPropertyChange("IsRunning");
        }

        /// <summary>
        ///     Start the Text-to-Speech engine
        /// </summary>
        private void StartTts()
        {
            if (this._ttsEngine != null)
                return;

            this._ttsEngine = new SpeechSynthesizer();
            this.TtsVoices = this._ttsEngine.GetInstalledVoices().Where(v => v.Enabled).ToList();
            if (this._selectedTtsVoice == null)
            {
                this._selectedTtsVoice = this.TtsVoices != null && this.TtsVoices.Any() ? this.TtsVoices[0] : null;
                this.NotifyPropertyChange("SelectedTtsVoice");
            }

            this.ResetTtsVoice();
            this._ttsEngine.Volume = this.TtsVolume;

            try
            {
                this._ttsEngine.SetOutputToDefaultAudioDevice();
            }
            catch (Exception e)
            {
                throw new SpeechProcessorNoOutputDeviceException("Failed to set output to default audio device.", e);
            }
        }

        /// <summary>
        ///     Stop the Speech Recognition Engine
        /// </summary>
        private void Stop()
        {
            this.SetHotKeys(null);

            if (this._recognizer != null)
            {
                this.HookupRecognizerEvents(false);
                if (this._recognizer.AudioState != AudioState.Stopped)
                    this._recognizer.RecognizeAsyncCancel();
                this._recognizer.Dispose();
                this._recognizer = null;

                this.NotifyPropertyChange("IsRunning");
            }
        }

        /// <summary>
        ///     Restart the Speech Recognition Engine
        /// </summary>
        private void StopAndRestartRecog()
        {
            var wasRunning = this.IsRecognizerRunning;
            if (this.IsRecognizerRunning)
                this.Stop();

            if (wasRunning)
                this.Start();
        }

        /// <summary>
        ///     Stop the Text-to-Speech engine
        /// </summary>
        private void StopTts()
        {
            if (this._ttsEngine == null)
                return;

            this._ttsEngine.Dispose();
            this._ttsEngine = null;
        }

        #endregion
    }
}