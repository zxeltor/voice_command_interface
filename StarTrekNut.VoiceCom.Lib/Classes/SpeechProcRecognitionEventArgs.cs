namespace StarTrekNut.VoiceCom.Lib.Classes
{
    /// <summary>
    ///     Used to pass speech recognition events to the user window
    /// </summary>
    public class SpeechProcRecognitionEventArgs
    {
        #region Constructors and Destructors

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

        #endregion

        #region Public Properties

        /// <summary>
        ///     Message sent to the user window.
        /// </summary>
        public string RecognitionResultText { get; set; }

        #endregion
    }
}