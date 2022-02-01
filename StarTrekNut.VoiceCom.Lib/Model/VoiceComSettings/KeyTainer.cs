using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarTrekNut.VoiceCom.Lib.Model.VoiceComSettings
{
    public class KeyTainer : IEquatable<KeyTainer>
    {
        public System.Windows.Input.Key WindowsKey { get; private set; }
        public Guid Id { get; set; }

        public KeyTainer(System.Windows.Input.Key windowsKey)
        {
            this.WindowsKey = windowsKey;
            Id = Guid.NewGuid();
        }

        public override string ToString()
        {
            return WindowsKey.ToString();
        }

        public bool Equals(KeyTainer other)
        {
            return this.Id.Equals(other.Id);
        }
    }
}
