namespace StarTrekNut.VoiceCom.Lib.Classes
{
    /// <summary>
    ///     Used to pass speech recognition events to the user window
    /// </summary>
    public class SpeechProcRecognitionEventArgs
    {
        #region Constructors and Destructors

        private Model.LogEntryType _entryType = Model.LogEntryType.Info;

        public SpeechProcRecognitionEventArgs()
        {
        }

        /// <summary>
        ///     Init object with the message for the user window.
        /// </summary>
        /// <param name="recognitionResult">Message sent to the user window</param>
        public SpeechProcRecognitionEventArgs(string recognitionResult)
        {
            this.RecognitionResultText = recognitionResult;
        }

        public SpeechProcRecognitionEventArgs(string recognitionResult, Model.LogEntryType logEntryType)
        {
            this.RecognitionResultText = recognitionResult;
            this.LogEntryType = logEntryType;
        }

        #endregion

        #region Public Properties

        public Model.LogEntryType LogEntryType
        {
            get
            {
                return this._entryType;
            }
            set
            {
                this._entryType = value;
            }
        }

        /// <summary>
        ///     Message sent to the user window.
        /// </summary>
        public string RecognitionResultText { get; set; }

        #endregion
    }
}