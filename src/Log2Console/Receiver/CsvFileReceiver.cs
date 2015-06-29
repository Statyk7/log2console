using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Text;
using System.Windows.Forms;
using Log2Console.Log;
using Log2Console.Settings;


namespace Log2Console.Receiver
{
    /// <summary>
    /// This receiver watch a given file, like a 'tail' program, with one log event by line.
    /// Ideally the log events should use the log4j XML Schema layout.
    /// </summary>
    [Serializable]
    [DisplayName("CSV Log File")]
    public class CsvFileReceiver : BaseReceiver
    {
        [NonSerialized]
        private FileSystemWatcher _fileWatcher;
        [NonSerialized]
        private StreamReader _fileReader;
        [NonSerialized]
        private string _filename;

        private string _fileToWatch = String.Empty;        
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
        [DisplayName("Show from Beginning")]
        [Description("Show file contents from the beginning (not just newly appended lines)")]
        [DefaultValue(false)]
        public bool ShowFromBeginning
        {
            get { return _showFromBeginning; }
            set { _showFromBeginning = value; }
        }

        private FieldType[] _fieldList = new[]
                                             {
                                                 new FieldType(LogMessageField.SequenceNr, "sequence"),
                                                 new FieldType(LogMessageField.TimeStamp, "time"),
                                                 new FieldType(LogMessageField.Level, "level"),
                                                 new FieldType(LogMessageField.ThreadName, "thread"),
                                                 new FieldType(LogMessageField.CallSiteClass, "class"),
                                                 new FieldType(LogMessageField.CallSiteMethod, "method"),
                                                 new FieldType(LogMessageField.Message, "message"),
                                                 new FieldType(LogMessageField.Exception, "exception"),
                                                 new FieldType(LogMessageField.SourceFileName, "file")
                                             };

        [Category("Configuration")]
        [DisplayName("Field List")]
        [Description("Defines the type of each field")]
        public FieldType[] FieldList
        {
            get { return _fieldList; }
            set { _fieldList = value; }
        }

        [Category("Configuration")]
        [DisplayName("Read Header From File")]
        [Description("Read the Header or First List of the CSV File to Automatically determine the Field Types")]
        [DefaultValue(false)]
        public bool ReadHeaderFromFile { get; set; }

        private string _dateTimeFormat = "yyyy/MM/dd HH:mm:ss.fff";

        [Category("Configuration")]
        [DisplayName("Time Format")]
        [Description("Specifies the DateTime Format used to Parse the DateTime Field")]
        [DefaultValue("yyyy/MM/dd HH:mm:ss.fff")]
        public string DateTimeFormat
        {
            get { return _dateTimeFormat; }
            set { _dateTimeFormat = value; }
        }

        private string _quoteChar = "\"";

        [Category("Configuration")]
        [DisplayName("Quote Char")]
        [Description("If a field includes the delimiter, the whole field will be enclosed with a quote")]
        [DefaultValue("\"")]
        public string QuoteChar
        {
            get { return _quoteChar; }
            set { _quoteChar = value; }
        }

        private string _delimiter = ",";

        [Category("Configuration")]
        [DisplayName("Delimiter ")]
        [Description("The character used to delimit each field")]
        [DefaultValue(",")]
        public string Delimiter
        {
            get { return _delimiter; }
            set { _delimiter = value; }
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
@"<target name=""CsvLog"" 
        xsi:type=""File"" 
        fileName=""${basedir}/Logs/log.csv""
        archiveFileName=""${basedir}/Logs/Archives/log_{#}_${date:format=yyyy-MM-d_HH}.txt""
        archiveEvery=""Hour""
        archiveNumbering=""Sequence""
        maxArchiveFiles=""30000""
        concurrentWrites=""true""
        keepFileOpen=""false""            
        >
    <layout xsi:type=""CSVLayout"">
    <column name =""sequence"" layout =""${counter}"" />
    <column name=""time"" layout=""${date:format=yyyy/MM/dd HH\:mm\:ss.fff}"" />
    <column name=""level"" layout=""${level}""/>
    <column name=""thread"" layout=""${threadid}""/>
    <column name=""class"" layout =""${callsite:className=true:methodName=false:fileName=false:includeSourcePath=false}"" />
    <column name=""method"" layout =""${callsite:className=false:methodName=true:fileName=false:includeSourcePath=false}"" />
    <column name=""message"" layout=""${message}"" />
    <column name=""exception"" layout=""${exception:format=Message,Type,StackTrace}"" />
    <column name=""file"" layout =""${callsite:className=false:methodName=false:fileName=true:includeSourcePath=true}"" />
    </layout>
</target>";
            }
        }

        public override void Initialize()
        {
            if (String.IsNullOrEmpty(_fileToWatch))
                return;

            _fileReader =
                new StreamReader(new FileStream(_fileToWatch, FileMode.Open, FileAccess.Read, FileShare.ReadWrite));

            string path = Path.GetDirectoryName(_fileToWatch);
            _filename = Path.GetFileName(_fileToWatch);
            _fileWatcher = new FileSystemWatcher(path, _filename)
                               {NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.Size};
            _fileWatcher.Changed += OnFileChanged;
            _fileWatcher.EnableRaisingEvents = true;

            ComputeFullLoggerName();

            if (ReadHeaderFromFile)
                AutoConfigureHeader();

            if (!_showFromBeginning)
            {
                _fileReader.BaseStream.Seek(0, SeekOrigin.End);
                _fileReader.DiscardBufferedData();
            }
        }

        private void AutoConfigureHeader()
        {
            var line = _fileReader. ReadLine();            
            var fields = line.Split(new[] { Delimiter }, StringSplitOptions.None);
            bool headerValid = false;
            try
            {     
                var fieldList = new FieldType[fields.Length];
                for (int index = 0; index < fields.Length; index++)
                {
                    var field = fields[index];

                    if (UserSettings.Instance.CsvHeaderFieldTypes.ContainsKey(field))
                    {
                        fieldList[index] = UserSettings.Instance.CsvHeaderFieldTypes[field];
                        
                        //Note: This is a very basic check for a valid header. If any field is detected, the header
                        //is considered valid. This could be made more thorough. 
                        headerValid = true;
                    }
                    else
                        fieldList[index] = new FieldType(LogMessageField.Properties, field, field);                                        
                }

                if (headerValid)
                {
                    _fieldList = fieldList;
                }
                else
                {
                    MessageBox.Show("Could not Parse the Header: " + line, "Error Parsing CSV Header");
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(string.Format("Could not Parse the Header: {0}\n\rError: {1}", line, ex),
                                "Error Parsing CSV Header");
            }
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
            if ((_fileReader == null))
                return;

            if (_fileReader.BaseStream.Position > _fileReader.BaseStream.Length)
            {
                _fileReader.BaseStream.Seek(0, SeekOrigin.Begin);
                _fileReader.DiscardBufferedData();
            }

            // Get last added lines
            var logMsgs = new List<LogMessage>();
            List<string> fields;

            while ((fields = ReadLogEntry()) != null)
            {
                var logMsg = new LogMessage {ThreadName = string.Empty};

                if (fields.Count == FieldList.Length)
                {
                    ParseFields(ref logMsg, fields);
                    logMsgs.Add(logMsg);
                }
            }

            // Notify the UI with the set of messages
            Notifiable.Notify(logMsgs.ToArray());
        }

        private void ParseFields(ref LogMessage logMsg, List<string> fields)
        {
            for (int i = 0; i < FieldList.Length; i++)
            {
                var fieldType = _fieldList[i];
                string fieldValue = fields[i];
                try
                {
                    switch (fieldType.Field)
                    {
                        case LogMessageField.SequenceNr:
                            logMsg.SequenceNr = ulong.Parse(fieldValue);
                            break;
                        case LogMessageField.LoggerName:
                            logMsg.LoggerName = fieldValue;
                            break;
                        case LogMessageField.RootLoggerName:
                            logMsg.RootLoggerName = fieldValue;
                            break;
                        case LogMessageField.Level:
                            logMsg.Level = LogLevels.Instance[(LogLevel) Enum.Parse(typeof (LogLevel), fieldValue)];
                            //if (logMsg.Level == null)
                            //    throw new NullReferenceException("Cannot parse string: " + fieldValue);
                            break;
                        case LogMessageField.Message:
                            logMsg.Message = fieldValue;
                            break;
                        case LogMessageField.ThreadName:
                            logMsg.ThreadName = fieldValue;
                            break;
                        case LogMessageField.TimeStamp:
                            DateTime time;
                            DateTime.TryParseExact(fieldValue, DateTimeFormat, null, DateTimeStyles.None, out time);
                            logMsg.TimeStamp = time;
                            break;
                        case LogMessageField.Exception:
                            logMsg.ExceptionString = fieldValue;
                            break;
                        case LogMessageField.CallSiteClass:
                            logMsg.CallSiteClass = fieldValue;
                            logMsg.LoggerName = logMsg.CallSiteClass;
                            break;
                        case LogMessageField.CallSiteMethod:
                            logMsg.CallSiteMethod = fieldValue;
                            break;
                        case LogMessageField.SourceFileName:
                            fieldValue = fieldValue.Trim("()".ToCharArray());
                            //Detect the Line Nr
                            var fileNameFields = fieldValue.Split(new[] {":"}, StringSplitOptions.None);
                            if (fileNameFields.Length == 3)
                            {
                                uint line;
                                var lineNrString = fileNameFields[2];
                                if (uint.TryParse(lineNrString, out line))
                                    logMsg.SourceFileLineNr = line;

                                var fileName = fieldValue.Substring(0, fieldValue.Length - lineNrString.Length - 1);
                                logMsg.SourceFileName = fileName;
                            }
                            else
                                logMsg.SourceFileName = fieldValue;
                            break;
                        case LogMessageField.SourceFileLineNr:
                            logMsg.SourceFileLineNr = uint.Parse(fieldValue);
                            break;
                        case LogMessageField.Properties:
                            logMsg.Properties.Add(fieldType.Property, fieldValue);
                            break;
                    }
                }
                catch (Exception ex)
                {
                    var sb = new StringBuilder();
                    foreach (var field in fields)
                    {
                        sb.Append(field);
                        sb.Append(Delimiter);
                    }

                    logMsg = new LogMessage
                                 {
                                     SequenceNr = 0,
                                     LoggerName = "Log2Console",
                                     RootLoggerName = "Log2Console",
                                     Level = LogLevels.Instance[LogLevel.Error],
                                     Message = "Error Parsing Log Entry Line: " + sb,
                                     ThreadName = string.Empty,
                                     TimeStamp = DateTime.Now,
                                     ExceptionString = ex.Message + ex.StackTrace,
                                     CallSiteClass = string.Empty,
                                     CallSiteMethod = string.Empty,
                                     SourceFileName = string.Empty,
                                     SourceFileLineNr = 0,
                                 };
                    return;
                }
            }
        }
        
        private List<string> ReadLogEntry()
        {
            var finalFields = new List<string>();
            bool quoteDetected = false;
            StringBuilder quoteString = null;

            do
            {
                //If there is a log entry, that spans multiple lines, it will be surrounded by the Quote Character. 
                if(quoteDetected)
                {
                    quoteString.AppendLine();
                }

                var line = _fileReader.ReadLine();
                if (line == null)
                    return null;

                if (string.IsNullOrEmpty(line)) //Skip blank lines
                    continue;

                var fields = line.Split(new[] {Delimiter}, StringSplitOptions.None);
                                
                foreach (var nextField in fields)
                {
                    //First check for Quote Char in fields
                    if(!quoteDetected)
                    {
                        //See if there is a start quote
                        if ((nextField.Length > 0) && (nextField.Substring(0,1).Equals(QuoteChar)))
                        {
                            quoteString = new StringBuilder();
                            if ((nextField.Length > 1))
                            {
                                var fieldWithoutQuote = nextField.Substring(1, nextField.Length - 1);
                                quoteString.Append(fieldWithoutQuote);
                                quoteDetected = true;
                            }
                        }
                            //If not, simply add the field
                        else
                        {
                            finalFields.Add(nextField);
                        }
                    }
                        //Keep on concatenating the string until the end quote is detected
                    else
                    {
                        //See if the last character is a quote                        
                        if((nextField.Length > 0) && (nextField.Substring(nextField.Length-1,1).Equals(QuoteChar)))
                        {
                            var fieldWithoutQuote = nextField.Substring(0, nextField.Length - 1);
                            quoteString.Append(fieldWithoutQuote);
                            quoteDetected = false;
                            finalFields.Add(quoteString.ToString());
                        }
                            //No quote is detected, keep on adding the next field
                        else
                        {
                            quoteString.Append(nextField);
                            quoteString.Append(Delimiter);  //Since this is enclosed in the Quote Char's it is part of a string field, and not valid delimiter                            
                        }
                    }
                }

                //If this is a normal log entry, without any quotes, then check that the correct amount of fields is detected
                if(!quoteDetected && (finalFields.Count != FieldList.Length))
                    return null;

            } while (finalFields.Count < FieldList.Length); //If this is a multi line log, keep on reading the following lines

            return finalFields;
        }
    }
}
