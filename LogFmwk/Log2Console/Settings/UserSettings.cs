using System;
using System.IO;
using System.Xml;
using System.ComponentModel;
using System.Drawing;

using Log2Console.Log;
using System.Xml.Serialization;
using System.Runtime.Serialization.Formatters.Binary;


namespace Log2Console.Settings
{
	[Serializable]
    public sealed class UserSettings
	{
		internal static readonly Color DefaultTraceLevelColor = Color.Gray;
		internal static readonly Color DefaultDebugLevelColor = Color.Black;
		internal static readonly Color DefaultInfoLevelColor = Color.Green;
		internal static readonly Color DefaultWarnLevelColor = Color.Orange;
		internal static readonly Color DefaultErrorLevelColor = Color.Red;
		internal static readonly Color DefaultFatalLevelColor = Color.Purple;

		[NonSerialized]
		private const string SettingsFileName = "UserSettings.dat";

		[NonSerialized]
		private static UserSettings _instance = null;

        private bool _hideTaskbarIcon = false;
        private bool _notifyNewLogWhenHidden = true;
        private bool _alwaysOnTop = false;
		private uint _transparency = 100;
		private bool _highlightLogger = true;
		private bool _highlightLogMessages = true;
		private bool _autoScrollToLastLog = true;
		private bool _groupLogMessages = false;
		private int _messageCycleCount = 0;
		private Font _logListFont = null;
		private Font _logDetailFont = null;
		private Font _loggerTreeFont = null;

		private Color _traceLevelColor = DefaultTraceLevelColor;
		private Color _debugLevelColor = DefaultDebugLevelColor;
		private Color _infoLevelColor = DefaultInfoLevelColor;
		private Color _warnLevelColor = DefaultWarnLevelColor;
		private Color _errorLevelColor = DefaultErrorLevelColor;
		private Color _fatalLevelColor = DefaultFatalLevelColor;

		private LogLevelInfo _logLevelInfo;


		private UserSettings()
		{
			// Set default values
			_logLevelInfo = LogLevels.Instance[(int)LogLevel.Trace];
		}


		public static UserSettings Instance
		{
			get { return _instance; }
		}

        public static void Load()
		{
			if (!File.Exists(SettingsFileName))
			{
				_instance = new UserSettings();
				return;
			}

			using (FileStream fs = new FileStream(SettingsFileName, FileMode.Open))
			{
				if (fs.Length > 0)
				{
					BinaryFormatter bf = new BinaryFormatter();
					_instance = bf.Deserialize(fs) as UserSettings;
				}
				else
					_instance = new UserSettings();
			}
        }
        
        public void Save()
		{
			using (FileStream fs = new FileStream(SettingsFileName, FileMode.Create))
			{
				BinaryFormatter bf = new BinaryFormatter();
				bf.Serialize(fs, this);
			}
        }


        [Category("Appearance")]
        [Description("Hides the taskbar icon, only the tray icon will remain visible.")]
        [DisplayName("Hide Taskbar Icon")]
        public bool HideTaskbarIcon
        {
            get { return _hideTaskbarIcon; }
            set { _hideTaskbarIcon = value; }
        }

        [Category("Appearance")]
        [Description("The Log2Console window will remain on top of all other windows.")]
        [DisplayName("Always On Top")]
        public bool AlwaysOnTop
        {
            get { return _alwaysOnTop; }
            set { _alwaysOnTop = value; }
        }

        [Category("Appearance")]
        [Description("Select a transparency factor for the main window.")]
        public uint Transparency
        {
            get { return _transparency; }
            set { _transparency = Math.Max(10, Math.Min(100, value)); }
        }

		[Category("Appearance")]
		[Description("Highlight the Logger of the selected Log Message.")]
		[DisplayName("Highlight Logger")]
		public bool HighlightLogger
		{
			get { return _highlightLogger; }
			set { _highlightLogger = value; }
		}

		[Category("Appearance")]
		[Description("Highlight the Log Messages of the selected Logger.")]
		[DisplayName("Highlight Log Messages")]
		public bool HighlightLogMessages
		{
			get { return _highlightLogMessages; }
			set { _highlightLogMessages = value; }
		}

        [Category("Notification")]
        [Description("A balloon tip will be displayed when a new log message arrives and the window is hidden.")]
        [DisplayName("Notify New Log When Hidden")]
        public bool NotifyNewLogWhenHidden
        {
            get { return _notifyNewLogWhenHidden; }
            set { _notifyNewLogWhenHidden = value; }
        }

        [Category("Notification")]
        [Description("Automatically scroll to the last log message.")]
        [DisplayName("Auto Scroll to Last Log")]
        public bool AutoScrollToLastLog
        {
            get { return _autoScrollToLastLog; }
            set { _autoScrollToLastLog = value; }
        }


		[Category("Logging")]
		[Description("Groups the log messages based on the Logger Name.")]
		[DisplayName("Group Log Messages by Loggers")]
		public bool GroupLogMessages
		{
			get { return _groupLogMessages; }
			set { _groupLogMessages = value; }
		}

		[Category("Logging")]
		[Description("When greater than 0, the log messages are limited to that number.")]
		[DisplayName("Message Cycle Count")]
		public int MessageCycleCount
		{
			get { return _messageCycleCount; }
			set { _messageCycleCount = value; }
		}

		[Category("Fonts")]
		[Description("Set the Font of the Log List View.")]
		[DisplayName("Log List View Font")]
		public Font LogListFont
		{
			get { return _logListFont; }
			set { _logListFont = value; }
		}

		[Category("Fonts")]
		[Description("Set the Font of the Log Detail View.")]
		[DisplayName("Log Detail View Font")]
		public Font LogDetailFont
		{
			get { return _logDetailFont; }
			set { _logDetailFont = value; }
		}

		[Category("Fonts")]
		[Description("Set the Font of the Logger Tree.")]
		[DisplayName("Logger Tree Font")]
		public Font LoggerTreeFont
		{
			get { return _loggerTreeFont; }
			set { _loggerTreeFont = value; }
		}


		[Category("Log Level Colors")]
		[DisplayName("1 - Trace Level Color")]
		public Color TraceLevelColor
		{
			get { return _traceLevelColor; }
			set { _traceLevelColor = value; }
		}

		[Category("Log Level Colors")]
		[DisplayName("2 - Debug Level Color")]
		public Color DebugLevelColor
		{
			get { return _debugLevelColor; }
			set { _debugLevelColor = value; }
		}

		[Category("Log Level Colors")]
		[DisplayName("3 - Info Level Color")]
		public Color InfoLevelColor
		{
			get { return _infoLevelColor; }
			set { _infoLevelColor = value; }
		}

		[Category("Log Level Colors")]
		[DisplayName("4 - Warning Level Color")]
		public Color WarnLevelColor
		{
			get { return _warnLevelColor; }
			set { _warnLevelColor = value; }
		}

		[Category("Log Level Colors")]
		[DisplayName("5 - Error Level Color")]
		public Color ErrorLevelColor
		{
			get { return _errorLevelColor; }
			set { _errorLevelColor = value; }
		}

		[Category("Log Level Colors")]
		[DisplayName("6 - Fatal Level Color")]
		public Color FatalLevelColor
		{
			get { return _fatalLevelColor; }
			set { _fatalLevelColor = value; }
		}


		/// <summary>
		/// This setting is not available through the Settings PropertyGrid.
		/// </summary>
		internal LogLevelInfo LogLevelInfo
		{
			get { return _logLevelInfo; }
			set { _logLevelInfo = value; }
		}

    }

}
