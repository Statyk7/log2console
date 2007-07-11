using System;
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
	public struct LogLevelInfo
	{
		public LogLevel Level;
		public string Name;
		public Color Color;
		public int RangeMin;
		public int RangeMax;


		public LogLevelInfo(LogLevel level, Color color)
		{
			Level = level;
			Name = level.ToString();
			Color = color;
			RangeMax = RangeMin = 0;
		}

		public LogLevelInfo(LogLevel level, string name, Color color, int rangeMin, int rangeMax)
		{
			Level = level;
			Name = name;
			Color = color;
			RangeMin = rangeMin;
			RangeMax = rangeMax;
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
				new LogLevelInfo(LogLevel.Trace, "Trace", UserSettings.DefaultTraceLevelColor, 0, 10000),
				new LogLevelInfo(LogLevel.Debug, "Debug", UserSettings.DefaultDebugLevelColor, 10001, 30000),
				new LogLevelInfo(LogLevel.Info, "Info", UserSettings.DefaultInfoLevelColor, 30001, 40000),
				new LogLevelInfo(LogLevel.Warn, "Warning", UserSettings.DefaultWarnLevelColor, 40001, 60000),
				new LogLevelInfo(LogLevel.Error, "Error", UserSettings.DefaultErrorLevelColor, 60001, 70000),
				new LogLevelInfo(LogLevel.Fatal, "Fatal", UserSettings.DefaultFatalLevelColor, 70001, 110000),
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
			get {
				if ((level < (int)LogLevel.Trace) || (level > (int)LogLevel.Fatal))
					return InvalidLogLevel;
				return LogLevelInfos[level];
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

    public class LogMessage
	{
		/// <summary>
		/// Logger Name.
		/// </summary>
		public string LoggerName;
		/// <summary>
		/// Log Level.
		/// </summary>
		public int Level;
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
    }
}
