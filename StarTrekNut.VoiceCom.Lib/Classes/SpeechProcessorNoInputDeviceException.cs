using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarTrekNut.VoiceCom.Lib.Classes
{
    public class SpeechProcessorNoInputDeviceException : Exception
    {
        public SpeechProcessorNoInputDeviceException(string message, Exception exception) : base(message, exception)
        {

        }
    }
}
