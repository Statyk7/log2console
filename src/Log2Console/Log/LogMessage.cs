using System;
using System.Collections.Generic;
using System.Text;
using Log2Console.Settings;


namespace Log2Console.Log
{
    public class LogMessage
    {
        /// <summary>
        /// The Line Number of the Log Message
        /// </summary>
        public ulong SequenceNr;

        /// <summary>
        /// Logger Name.
        /// </summary>
        public string LoggerName;

        /// <summary>
        /// Root Logger Name.
        /// </summary>
        public string RootLoggerName;

        /// <summary>
        /// Log Level.
        /// </summary>
        public LogLevelInfo Level;

        /// <summary>
        /// Log Message.
        /// </summary>
        public string Message;

        /// <summary>
        /// Thread Name.
        /// </summary>
        public string ThreadName;

        /// <summary>
        /// Time Stamp.
        /// </summary>
        public DateTime TimeStamp;

        /// <summary>
        /// Properties collection.
        /// </summary>
        public Dictionary<string, string> Properties = new Dictionary<string, string>();

        /// <summary>
        /// An exception message to associate to this message.
        /// </summary>
        public string ExceptionString;

        /// <summary>
        /// The CallSite Class
        /// </summary>
        public string CallSiteClass;


        /// <summary>
        /// The CallSite Method in which the Log is made
        /// </summary>
        public string CallSiteMethod;

        /// <summary>
        /// The Name of the Source File
        /// </summary>
        public string SourceFileName;

        /// <summary>
        /// The Line of the Source File
        /// </summary>
        public uint SourceFileLineNr;

        public void CheckNull()
        {
            if (string.IsNullOrEmpty(LoggerName))
                LoggerName = "Unknown";
            if (string.IsNullOrEmpty(RootLoggerName))
                RootLoggerName = "Unknown";
            if (string.IsNullOrEmpty(Message))
                Message = "Unknown";
            if (string.IsNullOrEmpty(ThreadName))
                ThreadName = string.Empty;
            if (string.IsNullOrEmpty(ExceptionString))
                ExceptionString = string.Empty;
            if (string.IsNullOrEmpty(ExceptionString))
                ExceptionString = string.Empty;
            if (string.IsNullOrEmpty(CallSiteClass))
                CallSiteClass = string.Empty;
            if (string.IsNullOrEmpty(CallSiteMethod))
                CallSiteMethod = string.Empty;
            if (string.IsNullOrEmpty(SourceFileName))
                SourceFileName = string.Empty;
            if (Level == null)
                Level = LogLevels.Instance[(LogLevel.Error)];
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            foreach (var fieldType in UserSettings.Instance.ColumnConfiguration)
            {
                sb.Append(GetInformation(fieldType));
                sb.Append("\t");
            }
            return sb.ToString();
        }

        private string GetInformation(FieldType fieldType)
        {
            string result = string.Empty;
            switch (fieldType.Field)
            {
                case LogMessageField.SequenceNr:
                    result = SequenceNr.ToString();
                    break;
                case LogMessageField.LoggerName:
                    result = LoggerName;
                    break;
                case LogMessageField.RootLoggerName:
                    result = RootLoggerName;
                    break;
                case LogMessageField.Level:
                    result = Level.Level.ToString();
                    break;
                case LogMessageField.Message:
                    result = Message;
                    break;
                case LogMessageField.ThreadName:
                    result = ThreadName;
                    break;
                case LogMessageField.TimeStamp:
                    result = TimeStamp.ToString(UserSettings.Instance.TimeStampFormatString);
                    break;
                case LogMessageField.Exception:
                    result = ExceptionString;
                    break;
                case LogMessageField.CallSiteClass:
                    result = CallSiteClass;
                    break;
                case LogMessageField.CallSiteMethod:
                    result = CallSiteMethod;
                    break;
                case LogMessageField.SourceFileName:
                    result = SourceFileName;
                    break;
                case LogMessageField.SourceFileLineNr:
                    result = SourceFileLineNr.ToString();
                    break;
                case LogMessageField.Properties:
                    result = Properties.ToString();
                    break;
            }
            return result;
        }

        public string GetMessageDetails()
        {
            var sb = new StringBuilder();
            sb.Append(@"{\rtf1\ansi ");
            foreach (var fieldType in UserSettings.Instance.MessageDetailConfiguration)
            {
                var info = GetInformation(fieldType);
                sb.Append(@"\b " + fieldType.Field + @": \b0 ");
                if (info.Length > 40)
                    sb.Append(@" \line ");
                sb.Append(info + @" \line ");
            }
            sb.Append(@"}");
            return sb.ToString();
        }
    }
}
