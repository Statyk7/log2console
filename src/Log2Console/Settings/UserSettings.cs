using System;
using System.Collections.Generic;
using System.IO;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Forms;

using Log2Console.Log;
using Log2Console.Receiver;


namespace Log2Console.Settings
{
    [Serializable]
    public sealed class LayoutSettings
    {
        public Rectangle WindowPosition { get; set; }
        public FormWindowState WindowState { get; set; }

        public bool ShowLogDetailView { get; set; }
        public bool ShowLoggerTree { get; set; }

        public void Initialize(Rectangle position, FormWindowState state, bool detailView, bool loggerTree)
        {
            WindowPosition = position;
            WindowState = state;
            ShowLogDetailView = detailView;
            ShowLoggerTree = loggerTree;
        }
    }


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
		private static UserSettings _instance;

        private bool _recursivlyEnableLoggers = false;
        private bool _hideTaskbarIcon = false;
        private bool _notifyNewLogWhenHidden = true;
        private bool _alwaysOnTop = false;
		private uint _transparency = 100;
		private bool _highlightLogger = true;
		private bool _highlightLogMessages = true;
		private bool _autoScrollToLastLog = true;
		private bool _groupLogMessages = false;
		private int _messageCycleCount = 0;
		private string _timeStampFormatString = "G";
		
		private Font _logListFont = null;
		private Font _logDetailFont = null;
		private Font _loggerTreeFont = null;

	    private Color _logListBackColor = Color.Empty;

		private Color _traceLevelColor = DefaultTraceLevelColor;
		private Color _debugLevelColor = DefaultDebugLevelColor;
		private Color _infoLevelColor = DefaultInfoLevelColor;
		private Color _warnLevelColor = DefaultWarnLevelColor;
		private Color _errorLevelColor = DefaultErrorLevelColor;
		private Color _fatalLevelColor = DefaultFatalLevelColor;

	    private bool _msgDetailsProperties = false;
        private bool _msgDetailsException = true;

        private LogLevelInfo _logLevelInfo;
        private List<IReceiver> _receivers = new List<IReceiver>();
	    private LayoutSettings _layout = new LayoutSettings();


		private UserSettings()
		{
			// Set default values
			_logLevelInfo = LogLevels.Instance[(int)LogLevel.Trace];
		}

        /// <summary>
        /// Creates and returns an exact copy of the settings.
        /// </summary>
        /// <returns></returns>
        public UserSettings Clone()
        {
            // We're going to serialize and deserialize to make the copy. That
            // way if we add new properties and/or settings, we don't have to 
            // maintain a copy constructor.
            BinaryFormatter formatter = new BinaryFormatter();

            using (MemoryStream ms = new MemoryStream())
            {
                // Serialize the object.
                formatter.Serialize(ms, this);

                // Reset the stream and deserialize it.
                ms.Position = 0;

                return formatter.Deserialize(ms) as UserSettings;
            }
        }

		public static UserSettings Instance
		{
			get { return _instance; }
            set { _instance = value; }
		}

        public static bool Load()
        {
            bool ok = false;

            _instance = new UserSettings();

        	string settingsFilePath = GetSettingsFilePath();
			if (!File.Exists(settingsFilePath))
				return ok;

            try
            {
				using (FileStream fs = new FileStream(settingsFilePath, FileMode.Open))
                {
                    if (fs.Length > 0)
                    {
                        BinaryFormatter bf = new BinaryFormatter();
                        _instance = bf.Deserialize(fs) as UserSettings;
                        
                        // During 1st load, receiver collection is set to null...
                        if ((_instance != null) && (_instance._receivers == null))
							_instance._receivers = new List<IReceiver>();

                        // During 1st load, layout is set to null...
                        if ((_instance != null) && (_instance._layout == null))
							_instance._layout = new LayoutSettings();

                        ok = true;
                    }
                }
            }
            catch (Exception)
            {
                // The settings file might be corrupted or from too different version, delete it...
                try
                {
					File.Delete(settingsFilePath);
                }
                catch
                {
                    ok = false;
                }
            }

            return ok;
		}

		private static string GetSettingsFilePath()
		{
			string userDir = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
			
			DirectoryInfo di = new DirectoryInfo(userDir);
			di = di.CreateSubdirectory("Log2Console");

			return di.FullName + Path.DirectorySeparatorChar + SettingsFileName;
		}

	    public void Save()
		{
			string settingsFilePath = GetSettingsFilePath();

			using (FileStream fs = new FileStream(settingsFilePath, FileMode.Create))
			{
				BinaryFormatter bf = new BinaryFormatter();
				bf.Serialize(fs, this);
			}
        }

        public void Close()
        {
            foreach (IReceiver receiver in _receivers)
            {
                receiver.Detach();
                receiver.Terminate();
            }
            _receivers.Clear();
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

		[Category("Logging")]
		[Description("Defines the format to be used to display the log message timestamps (cf. DateTime.ToString(format) in the .NET Framework.")]
		[DisplayName("TimeStamp Format String")]
		public string TimeStampFormatString
		{
			get { return _timeStampFormatString; }
			set
			{
				// Check validity
				try
				{
					string str= DateTime.Now.ToString(value); // If error, will throw FormatException
					_timeStampFormatString = value;
				}
				catch (FormatException ex)
				{
					MessageBox.Show(Form.ActiveForm, ex.Message, Form.ActiveForm.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
					_timeStampFormatString = "G"; // Back to default
				}
			}
		}

        [Category("Logging")]
        [Description("When a logger is enabled or disabled, do the same for all child loggers.")]
        [DisplayName("Recursively Enable Loggers")]
        public bool RecursivlyEnableLoggers
        {
            get { return _recursivlyEnableLoggers; }
            set { _recursivlyEnableLoggers = value; }
        }


        [Category("Message Details")]
        [Description("Show or hide the message properties in the message details panel.")]
        [DisplayName("Show Properties")]
        public bool ShowMsgDetailsProperties
        {
            get { return _msgDetailsProperties; }
            set { _msgDetailsProperties = value; }
        }

        [Category("Message Details")]
        [Description("Show or hide the exception in the message details panel.")]
        [DisplayName("Show Exception")]
        public bool ShowMsgDetailsException
        {
            get { return _msgDetailsException; }
            set { _msgDetailsException = value; }
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

		[Category("Colors")]
		[Description("Set the Background Color of the Log List View.")]
		[DisplayName("Log List View Background Color")]
        public Color LogListBackColor
		{
            get { return _logListBackColor; }
            set { _logListBackColor = value; }
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
		[Browsable(false)]
		internal LogLevelInfo LogLevelInfo
		{
			get { return _logLevelInfo; }
			set { _logLevelInfo = value; }
		}

		/// <summary>
		/// This setting is not available through the Settings PropertyGrid.
        /// </summary>
        [Browsable(false)]
        internal List<IReceiver> Receivers
		{
            get { return _receivers; }
            set { _receivers = value; }
		}

		/// <summary>
		/// This setting is not available through the Settings PropertyGrid.
        /// </summary>
        [Browsable(false)]
        internal LayoutSettings Layout
		{
            get { return _layout; }
            set { _layout = value; }
		}
    }
}
