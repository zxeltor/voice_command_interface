using System;

namespace StarTrekNut.VoiceCom.Lib.Classes
{
    public class SpeechProcessorNoInputDeviceException : Exception
    {
        public SpeechProcessorNoInputDeviceException(string message, Exception exception) : base(message, exception)
        {

        }
    }
}
