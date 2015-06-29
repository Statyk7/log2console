using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text;

using Log2Console.Log;


namespace Log2Console.Receiver
{
    /// <summary>
    /// This receiver watch a given file, like a 'tail' program, with one log event by line.
    /// Ideally the log events should use the log4j XML Schema layout.
    /// </summary>
    [Serializable]
    [DisplayName("Log File (Flat or Log4j XML Formatted)")]
    public class FileReceiver : BaseReceiver
    {
        public enum FileFormatEnums
        {
            Log4jXml,
            Flat,
        }


        [NonSerialized]
        private FileSystemWatcher _fileWatcher;
        [NonSerialized]
        private StreamReader _fileReader;
        [NonSerialized]
        private long _lastFileLength;
        [NonSerialized]
        private string _filename;
        [NonSerialized]
        private string _fullLoggerName;

        private string _fileToWatch = String.Empty;
        private FileFormatEnums _fileFormat;
        private bool _showFromBeginning;
        private string _loggerName;


        [Category("Configuration")]
        [DisplayName("File to Watch")]
        public string FileToWatch
        {
            get { return _fileToWatch; }
            set
            {
                if (String.Compare(_fileToWatch, value, true) == 0)
                    return;

                _fileToWatch = value;

                Restart();
            }
        }

        [Category("Configuration")]
        [DisplayName("File Format (Flat or Log4j XML)")]
        public FileFormatEnums FileFormat
        {
            get { return _fileFormat; }
            set { _fileFormat = value; }
        }
        
        [Category("Configuration")]
        [DisplayName("Show from Beginning")]
        [Description("Show file contents from the beginning (not just newly appended lines)")]
        [DefaultValue(false)]
        public bool ShowFromBeginning
        {
            get { return _showFromBeginning; }
            set
            {
                _showFromBeginning = value;
 
                if (value && _lastFileLength == 0)
                {
                    ReadFile();
                }
            }
        }

        [Category("Behavior")]
        [DisplayName("Logger Name")]
        [Description("Append the given Name to the Logger Name. If left empty, the filename will be used.")]
        public string LoggerName
        {
            get { return _loggerName; }
            set
            {
                _loggerName = value;

                ComputeFullLoggerName();
            }
        }


        #region IReceiver Members

        [Browsable(false)]
        public override string SampleClientConfig
        {
            get
            {
                return
                    "Configuration for log4net:" + Environment.NewLine +
                    "<appender name=\"FileAppender\" type=\"log4net.Appender.FileAppender\">" + Environment.NewLine +
                    "    <file value=\"log-file.txt\" />" + Environment.NewLine +
                    "    <appendToFile value=\"true\" />" + Environment.NewLine +
                    "    <lockingModel type=\"log4net.Appender.FileAppender+MinimalLock\" />" + Environment.NewLine +
                    "    <layout type=\"log4net.Layout.XmlLayoutSchemaLog4j\" />" + Environment.NewLine +
                    "</appender>";
            }
        }

        public override void Initialize()
        {
            if (String.IsNullOrEmpty(_fileToWatch))
                return;

            _fileReader =
                new StreamReader(new FileStream(_fileToWatch, FileMode.Open, FileAccess.Read, FileShare.ReadWrite));

            _lastFileLength = _showFromBeginning ? 0 : _fileReader.BaseStream.Length;

            string path = Path.GetDirectoryName(_fileToWatch);
            _filename = Path.GetFileName(_fileToWatch);
            _fileWatcher = new FileSystemWatcher(path, _filename);
            _fileWatcher.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.Size;
            _fileWatcher.Changed += OnFileChanged;
            _fileWatcher.EnableRaisingEvents = true;

            ComputeFullLoggerName();
        }

        public override void Terminate()
        {
            if (_fileWatcher != null)
            {
                _fileWatcher.EnableRaisingEvents = false;
                _fileWatcher.Changed -= OnFileChanged;
                _fileWatcher = null;
            }

            if (_fileReader != null)
                _fileReader.Close();
            _fileReader = null;

            _lastFileLength = 0;
        }
        
        public override void Attach(ILogMessageNotifiable notifiable)
        {
            base.Attach(notifiable);
            
            if (_showFromBeginning)
                ReadFile();
        }

        #endregion


        private void Restart()
        {
            Terminate();
            Initialize();
        }

        private void ComputeFullLoggerName()
        {
            _fullLoggerName = String.Format("FileLogger.{0}",
                                            String.IsNullOrEmpty(_loggerName)
                                                ? _filename.Replace('.', '_')
                                                : _loggerName);

            DisplayName = String.IsNullOrEmpty(_loggerName)
                              ? String.Empty
                              : String.Format("Log File [{0}]", _loggerName);
        }

        private void OnFileChanged(object sender, FileSystemEventArgs e)
        {
            if (e.ChangeType != WatcherChangeTypes.Changed)
                return;

            ReadFile();
        }

        private void ReadFile()
        {
            if ((_fileReader == null) || (_fileReader.BaseStream.Length == _lastFileLength))
                return;

            // Seek to the last file length
            _fileReader.BaseStream.Seek(_lastFileLength, SeekOrigin.Begin);

            // Get last added lines
            string line;
            var sb = new StringBuilder();
            List<LogMessage> logMsgs = new List<LogMessage>();
            
            while ((line = _fileReader.ReadLine()) != null)
            {
                if (_fileFormat == FileFormatEnums.Flat)
                {
                    LogMessage logMsg = new LogMessage();
                    logMsg.RootLoggerName = _loggerName;
                    logMsg.LoggerName = _fullLoggerName;
                    logMsg.ThreadName = "NA";
                    logMsg.Message = line;
                    logMsg.TimeStamp = DateTime.Now;
                    logMsg.Level = LogLevels.Instance[LogLevel.Info];

                    logMsgs.Add(logMsg);
                }
                else
                {
                    sb.Append(line);

                    // This condition allows us to process events that spread over multiple lines
                    if (line.Contains("</log4j:event>"))
                    {
                        LogMessage logMsg = ReceiverUtils.ParseLog4JXmlLogEvent(sb.ToString(), _fullLoggerName);
                        logMsgs.Add(logMsg);
                        sb = new StringBuilder();
                    }
                }
            }

            // Notify the UI with the set of messages
            Notifiable.Notify(logMsgs.ToArray());

            // Update the last file length
            _lastFileLength = _fileReader.BaseStream.Position;
        }
    }
}
