using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using System.Xml.Serialization;

namespace StarTrekNut.VoiceCom.Lib.Model.VoiceComSettings
{
    public class HotKey : INotifyPropertyChanged
    {
        #region Fields

        private Key _key;

        #endregion

        #region Public Events

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Public Properties

        //public List<Key> KeyList
        //{
        //    get => this._keyList;
        //    set
        //    {
        //        this._keyList = value;
        //        this.NotifyPropertyChange("KeyList");
        //    }
        //}

        //[XmlIgnore]
        //public string Keys
        //{
        //    get
        //    {
        //        var result = string.Empty;

        //        for (var intI = 0; intI < this.KeyList.Count; intI++)
        //        {
        //            if (intI < this.KeyList.Count - 1)
        //                result = result + this.KeyList[intI] + "+";
        //            else
        //                result = result + this.KeyList[intI];
        //        }

        //        return result;
        //    }
        //}
        #endregion

        #region Public Methods and Operators

        //public Key? GetKey()
        //{
        //    if (this.KeyList != null && this.KeyList.Any())
        //        return this.KeyList.FirstOrDefault();

        //    return null;
        //}

        #endregion

        #region Methods

        private void NotifyPropertyChange(string propertyName)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}