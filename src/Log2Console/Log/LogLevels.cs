using Log2Console.Settings;
using System;
using System.Drawing;

namespace Log2Console.Log
{
    public sealed class LogLevels
    {
        private static LogLevels _instance;

        public readonly LogLevelInfo InvalidLogLevel;
        public readonly LogLevelInfo[] LogLevelInfos;

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
    }
}
