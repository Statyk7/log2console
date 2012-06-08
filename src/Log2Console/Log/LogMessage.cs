using System;
using System.Collections.Generic;
using System.Drawing;
using Log2Console.Settings;


namespace Log2Console.Log
{
  [Serializable]
  public enum LogLevel
  {
    None = -1,
    Trace,
    Debug,
    Info,
    Warn,
    Error,
    Fatal
  }

  [Serializable]
  public class LogLevelInfo
  {
    public LogLevel Level;
    public string Name;
    public Color Color;
    public int Value;
    public int RangeMin;
    public int RangeMax;


    public LogLevelInfo(LogLevel level, Color color)
    {
      Level = level;
      Name = level.ToString();
      Color = color;
      RangeMax = RangeMin = 0;
    }

    public LogLevelInfo(LogLevel level, string name, Color color, int value, int rangeMin, int rangeMax)
    {
      Level = level;
      Name = name;
      Color = color;
      Value = value;
      RangeMin = rangeMin;
      RangeMax = rangeMax;
    }

    public override bool Equals(object obj)
    {
      if (obj is LogLevelInfo)
        return ((obj as LogLevelInfo) == this);
      return base.Equals(obj);
    }

    public override int GetHashCode()
    {
      return Value.GetHashCode();
    }

    public static bool operator ==(LogLevelInfo first, LogLevelInfo second)
    {
        if (((object) first == null) || ((object) second == null))
            return (((object) first == null) && ((object) second == null));
        return (first.Value == second.Value);
    }

    public static bool operator !=(LogLevelInfo first, LogLevelInfo second)
    {
        if (((object) first == null) || ((object) second == null))
            return !(((object) first == null) && ((object) second == null));
        return first.Value != second.Value;
    }
  }


  public sealed class LogLevels
  {
    private static LogLevels _instance = null;

    public readonly LogLevelInfo InvalidLogLevel;
    public readonly LogLevelInfo[] LogLevelInfos;

    private LogLevels()
    {
      InvalidLogLevel = new LogLevelInfo(LogLevel.None, Color.IndianRed);

      LogLevelInfos = new LogLevelInfo[]
      {
        new LogLevelInfo(LogLevel.Trace, "Trace", UserSettings.DefaultTraceLevelColor, 10000, 0, 10000),
        new LogLevelInfo(LogLevel.Debug, "Debug", UserSettings.DefaultDebugLevelColor, 30000, 10001, 30000),
        new LogLevelInfo(LogLevel.Info, "Info", UserSettings.DefaultInfoLevelColor, 40000, 30001, 40000),
        new LogLevelInfo(LogLevel.Warn, "Warn", UserSettings.DefaultWarnLevelColor, 60000, 40001, 60000),
        new LogLevelInfo(LogLevel.Error, "Error", UserSettings.DefaultErrorLevelColor, 70000, 60001, 70000),
        new LogLevelInfo(LogLevel.Fatal, "Fatal", UserSettings.DefaultFatalLevelColor, 110000, 70001, 110000),
      };
    }

    internal static LogLevels Instance
    {
      get
      {
        if (_instance == null)
          _instance = new LogLevels();
        return _instance;
      }
    }

    internal LogLevelInfo this[int level]
    {
      get
      {
        if ((level < (int)LogLevel.Trace) || (level > (int)LogLevel.Fatal))
          return InvalidLogLevel;
        return LogLevelInfos[level];
      }
    }

    internal LogLevelInfo this[LogLevel logLevel]
    {
      get
      {
        int level = (int)logLevel;
        if ((level < (int)LogLevel.Trace) || (level > (int)LogLevel.Fatal))
          return InvalidLogLevel;
        return LogLevelInfos[level];
      }
    }

    internal LogLevelInfo this[string level]
    {
      get
      {
        foreach (LogLevelInfo info in LogLevelInfos)
        {
          if (info.Name.Equals(level, StringComparison.InvariantCultureIgnoreCase))
            return info;
        }
        return InvalidLogLevel;
      }
    }

  }

  public static class LogUtils
  {
    public static bool IsInRange(int val, int min, int max)
    {
      return (val >= min) && (val <= max);
    }

    public static LogLevelInfo GetLogLevelInfo(int level)
    {
      foreach (LogLevelInfo info in LogLevels.Instance.LogLevelInfos)
      {
        if (IsInRange(level, info.RangeMin, info.RangeMax))
          return info;
      }

      return LogLevels.Instance.InvalidLogLevel;
    }

    public static LogLevelInfo GetLogLevelInfo(LogLevel level)
    {
      foreach (LogLevelInfo info in LogLevels.Instance.LogLevelInfos)
      {
        if (level == info.Level)
          return info;
      }

      return LogLevels.Instance.InvalidLogLevel;
    }
  }


    public enum LogMessageField
    {
        SequenceNr,
        LoggerName,
        Level,
        Message,
        ThreadName,
        TimeStamp,
        Exception,
        CallSiteClass,
        CallSiteMethod,
        SourceFileName,
        SourceFileLineNr,
        Properties
    }

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
    }
}
