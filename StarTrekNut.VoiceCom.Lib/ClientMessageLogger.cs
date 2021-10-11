using System;
using System.Collections.ObjectModel;
using System.ComponentModel;

using StarTrekNut.VoiceCom.Lib.Model;

namespace StarTrekNut.VoiceCom.Lib
{
    /// <summary>
    ///     A helper class which handles the sending notifications to the user and log4net
    /// </summary>
    public class ClientMessageLogger : INotifyPropertyChanged
    {
        #region Static Fields

        private static ClientMessageLogger _instance;

        #endregion

        #region Fields

        #endregion

        #region Constructors and Destructors

        private ClientMessageLogger()
        {
            if (this.LogEntries == null)
            {
                this.LogEntries = new ObservableCollection<LogEntry>();
                this.NotifyPropertyChange("LogEntries");
            }

            

            this.MaxLogEntriesToDisplay = 250;
        }

        #endregion

        #region Public Events

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Public Properties

        /// <summary>
        ///     Get the singleton instance of this class
        /// </summary>
        public static ClientMessageLogger Instance => _instance ?? (_instance = new ClientMessageLogger());

        /// <summary>
        ///     A collection of log entries used for databinding in the user window.
        /// </summary>
        public ObservableCollection<LogEntry> LogEntries { get; }

        #endregion

        #region Properties

        /// <summary>
        ///     Maximum number of entries to show in the user window
        /// </summary>
        private int MaxLogEntriesToDisplay { get; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Send error log entry to log4net, and adds a log entry to the user window.
        /// </summary>
        /// <param name="message">The message</param>
        /// <param name="exception">An exception is one is available.</param>
        public void Error(string message, Exception exception = null)
        {
            //if (exception == null)
            //    System.Diagnostics.EventLog.WriteEntry("VoiceCommInt", message, System.Diagnostics.EventLogEntryType.Error);
            //else
            //    System.Diagnostics.EventLog.WriteEntry("VoiceCommInt", $"{message}: {exception}", System.Diagnostics.EventLogEntryType.Error);

            this.AddLogEntry(new LogEntry { EntryType = LogEntryType.Error, EntryDateTime = DateTime.Now, EntryMessage = message });
        }

        /// <summary>
        ///     Send info log entry to log4net, and adds a log entry to the user window.
        /// </summary>
        /// <param name="message">The message</param>
        /// <param name="exception">An exception is one is available.</param>
        public void Info(string message)
        {
            //System.Diagnostics.EventLog.WriteEntry("VoiceCommInt", message, System.Diagnostics.EventLogEntryType.Information);

            this.AddLogEntry(new LogEntry { EntryType = LogEntryType.Info, EntryDateTime = DateTime.Now, EntryMessage = message });
        }

        /// <summary>
        ///     Send warning log entry to log4net, and adds a log entry to the user window.
        /// </summary>
        /// <param name="message">The message</param>
        /// <param name="exception">An exception is one is available.</param>
        public void Warning(string message, Exception exception = null)
        {
            //if (exception == null)
            //    _log4NetLogger.Warn(message);
            //else
            //    _log4NetLogger.Warn(message, exception);

            this.AddLogEntry(new LogEntry { EntryType = LogEntryType.Warning, EntryDateTime = DateTime.Now, EntryMessage = message });
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Addes a log entry to the LogEntries collection
        /// </summary>
        /// <param name="logEntry">Log entry to add</param>
        private void AddLogEntry(LogEntry logEntry)
        {
            if (this.LogEntries.Count == this.MaxLogEntriesToDisplay)
                this.LogEntries.RemoveAt(0);
            else if (this.LogEntries.Count > this.MaxLogEntriesToDisplay)
                for (var intI = 0; intI < this.LogEntries.Count - this.MaxLogEntriesToDisplay; intI++)
                {
                    this.LogEntries.RemoveAt(0);
                }

            this.LogEntries.Add(logEntry);
        }

        private void NotifyPropertyChange(string propertyName)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}