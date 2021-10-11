using System;
using System.ComponentModel;

namespace StarTrekNut.VoiceCom.Lib.Model
{
    /// <summary>
    ///     A helper class to combine sending information to the UI and log4net
    /// </summary>
    public class LogEntry : INotifyPropertyChanged
    {
        #region Fields

        private DateTime _entryDateTime;

        private string _entryMessage;

        private LogEntryType _entryType;

        #endregion

        #region Public Events

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Public Properties

        /// <summary>
        ///     Date and time of this entry
        /// </summary>
        public DateTime EntryDateTime
        {
            get => this._entryDateTime;
            set
            {
                this._entryDateTime = value;
                this.NotifyPropertyChange("EntryDateTime");
            }
        }

        /// <summary>
        ///     The message for this entry
        /// </summary>
        public string EntryMessage
        {
            get => this._entryMessage;
            set
            {
                this._entryMessage = value;
                this.NotifyPropertyChange("EntryMessage");
            }
        }

        /// <summary>
        ///     The log entry type as defined by the LogEntryType enum
        /// </summary>
        public LogEntryType EntryType
        {
            get => this._entryType;
            set
            {
                this._entryType = value;
                this.NotifyPropertyChange("EntryType");
            }
        }

        #endregion

        #region Methods

        private void NotifyPropertyChange(string propertyName)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }

    /// <summary>
    ///     An enum which defines several log entry types.
    /// </summary>
    public enum LogEntryType
    {
        Unknown,

        Info,

        Warning,

        Error,

        Debug
    }
}