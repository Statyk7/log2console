using System;
using System.ComponentModel;
using System.Diagnostics;

using Log2Console.Log;


namespace Log2Console.Receiver
{
    [Serializable]
    [DisplayName("Windows Event Log")]
    public class EventLogReceiver : BaseReceiver
    {
        [NonSerialized]
        private EventLog _eventLog;

        private string _logName;
        private string _machineName = ".";
        private string _source;
        private bool _appendHostNameToLogger = true;


        [Category("Configuration")]
        [DisplayName("Event Log Name")]
        [Description("The name of the log on the specified computer.")]
        public string LogName
        {
            get { return _logName; }
            set { _logName = value; }
        }

        [Category("Configuration")]
        [DisplayName("Machine Name")]
        [Description("The computer on which the log exists.")]
        public string MachineName
        {
            get { return _machineName; }
            set { _machineName = value; }
        }

        [Category("Configuration")]
        [DisplayName("Event Log Source")]
        [Description("The source of event log entries.")]
        public string Source
        {
            get { return _source; }
            set { _source = value; }
        }

        [Category("Behavior")]
        [DisplayName("Append Machine Name to Logger")]
        [Description("Append the remote Machine Name to the Logger Name.")]
        public bool AppendHostNameToLogger
        {
            get { return _appendHostNameToLogger; }
            set { _appendHostNameToLogger = value; }
        }

        [NonSerialized]
        private string _baseLoggerName;


        #region Overrides of BaseReceiver

        [Browsable(false)]
        public override string SampleClientConfig
        {
            get
            {
                return "Use Log2Console to display the Windows Event Logs." + Environment.NewLine +
                       "Note that the Thread column is used to display the Instance ID (Event ID).";
            }
        }

        public override void Initialize()
        {
            if (String.IsNullOrEmpty(MachineName))
                MachineName = ".";

            _eventLog = new EventLog(LogName, MachineName, Source);
            _eventLog.EntryWritten += EventLogOnEntryWritten;
            _eventLog.EnableRaisingEvents = true;

            _baseLoggerName = AppendHostNameToLogger && !String.IsNullOrEmpty(MachineName) && (MachineName != ".")
                                    ? String.Format("[Host: {0}].{1}", MachineName, LogName)
                                    : LogName;

        }

        public override void Terminate()
        {
            if (_eventLog != null)
                _eventLog.Dispose();
            _eventLog = null;
        }

        #endregion


        private void EventLogOnEntryWritten(object sender, EntryWrittenEventArgs entryWrittenEventArgs)
        {
            LogMessage logMsg = new LogMessage();
            logMsg.RootLoggerName = _baseLoggerName;
            logMsg.LoggerName = String.IsNullOrEmpty(entryWrittenEventArgs.Entry.Source)
                                    ? _baseLoggerName
                                    : String.Format("{0}.{1}", _baseLoggerName, entryWrittenEventArgs.Entry.Source);

            logMsg.Message = entryWrittenEventArgs.Entry.Message;
            logMsg.TimeStamp = entryWrittenEventArgs.Entry.TimeGenerated;
            logMsg.Level = LogUtils.GetLogLevelInfo(GetLogLevel(entryWrittenEventArgs.Entry.EntryType));
            logMsg.ThreadName = entryWrittenEventArgs.Entry.InstanceId.ToString();

            if (!String.IsNullOrEmpty(entryWrittenEventArgs.Entry.Category))
                logMsg.Properties.Add("Category", entryWrittenEventArgs.Entry.Category);
            if (!String.IsNullOrEmpty(entryWrittenEventArgs.Entry.UserName))
                logMsg.Properties.Add("User Name", entryWrittenEventArgs.Entry.UserName);

            Notifiable.Notify(logMsg);
        }

        private static LogLevel GetLogLevel(EventLogEntryType entryType)
        {
            switch (entryType)
            {
                case EventLogEntryType.Warning: return LogLevel.Warn;
                case EventLogEntryType.FailureAudit:
                case EventLogEntryType.Error: return LogLevel.Error;
                case EventLogEntryType.SuccessAudit:
                case EventLogEntryType.Information: return LogLevel.Info;
                default:
                    return LogLevel.None;
            }
        }
    }
}
