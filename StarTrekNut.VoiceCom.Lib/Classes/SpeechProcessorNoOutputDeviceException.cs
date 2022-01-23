using System;

namespace StarTrekNut.VoiceCom.Lib.Classes
{
    public class SpeechProcessorNoOutputDeviceException : Exception
    {
        public SpeechProcessorNoOutputDeviceException(string message, Exception exception) : base(message, exception)
        {
            
        }
    }
}
