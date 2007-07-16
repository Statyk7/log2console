using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

using Log2Console.Settings;


namespace Log2Console.Log
{
	/// <summary>
	/// Describes a Logger.
	/// </summary>
	public class LoggerItem
	{
		private const char LoggerSeparator = '.';


		/// <summary>
		/// Parent Logger Item. Null for the Root.
		/// </summary>
		public LoggerItem Parent = null;
		/// <summary>
		/// Collection of child Logger Items, identified by their full path.
		/// </summary>
		public Dictionary<string, LoggerItem> Loggers = new Dictionary<string,LoggerItem>();
		/// <summary>
		/// Collection of Log Messages for this Logger.
		/// </summary>
		public List<LogMessageItem> LogMessages = new List<LogMessageItem>();

		/// <summary>
		/// The associated Tree Node.
		/// </summary>
		public TreeNode Node = null;
		/// <summary>
		/// A reference to the Logger TreeView associated to this Logger.
		/// </summary>
		private TreeView _loggerTreeView = null;
		/// <summary>
		/// The optional associated List View Group.
		/// </summary>
		public ListViewGroup Group = null;
		/// <summary>
		/// A reference to the Log ListView associated to this Logger.
		/// </summary>
		private ListView _logListView = null;

		/// <summary>
		/// Short Name of this Logger.
		/// </summary>
		public string Name = String.Empty;
		/// <summary>
		/// Full Name (or "Path") of this Logger.
		/// </summary>
		public string FullName = String.Empty;
		/// <summary>
		/// When set the Logger and its Messages are displayed.
		/// </summary>
		private bool _enabled = true;

		private bool _highlight = false;
		private bool _highlightLogMessages = false;


		private LoggerItem()
		{
		}

		public static LoggerItem CreateRootLoggerItem(string name, TreeView loggerTreeView, ListView logListView)
		{
			LoggerItem logger = new LoggerItem();
			logger.Name = name;
			logger._loggerTreeView = loggerTreeView;
			logger._logListView = logListView;

			// Tree Node
			logger.Node = new TreeNode(name);
			logger.Node.Checked = true;
			//logger._loggerTreeView.BeginUpdate();
			logger._loggerTreeView.Nodes.Add(logger.Node);
			//logger._loggerTreeView.EndUpdate();

			return logger;
		}

		private static LoggerItem CreateLoggerItem(string name, string fullName, LoggerItem parent)
		{
			if (parent == null)
				throw new ArgumentNullException();

			LoggerItem logger = new LoggerItem();
			logger.Name = name;
			logger.FullName = fullName;
			logger.Parent = parent;

			logger._loggerTreeView = logger.Parent._loggerTreeView;
			logger._logListView = logger.Parent._logListView;

			logger.Parent.Loggers.Add(name, logger);

			// Tree Node
			logger.Node = new TreeNode(name);
			logger.Node.Tag = logger;
			logger.Node.Checked = true;
			//logger._loggerTreeView.BeginUpdate();
			logger.Parent.Node.Nodes.Add(logger.Node);
			logger.Parent.Node.Expand();
			logger.Node.EnsureVisible();
			//logger._loggerTreeView.EndUpdate();

			// Group
			if (UserSettings.Instance.GroupLogMessages)
			{
				logger.Group = new ListViewGroup(fullName);
				logger._logListView.Groups.Add(logger.Group);
			}

			return logger;
		}


		internal bool Enabled
		{
			get { return _enabled; }
			set
			{
				if (_enabled == value)
					return;

				_enabled = value;

				if (LogMessages.Count == 0)
					return;

				// Update List View
				_logListView.BeginUpdate();
				if (_enabled)
				{
					for (int i = 0; i < LogMessages.Count; i++)
					{
						LogMessageItem item = LogMessages[i];
						if (item.IsLevelInRange())
							EnableLogMessage(item);
					}
				}
				else
				{
					for (int i = 0; i < LogMessages.Count; i++)
						DisableLogMessage(LogMessages[i]);
				}
				_logListView.EndUpdate();
			}
		}


		internal bool Highlight
		{
			get { return _highlight; }
			set
			{
				// Don't allow highlight for the root item
				if ((_highlight == value) || (Parent == null))
					return;

				_highlight = value;

				if (value)
					Node.BackColor = Color.LightBlue;
				else
					Node.BackColor = Color.Transparent;

				if (Parent != null)
					Parent.Highlight = value;
			}
		}


		internal bool HighlightLogMessages
		{
			get { return _highlightLogMessages; }
			set
			{
				// Don't allow highlight for the root item
				if (Parent == null)
					return;

				_highlightLogMessages = value;

				Color color = value ? Color.LightBlue : Color.Transparent;

				_logListView.BeginUpdate();
				foreach (LogMessageItem item in LogMessages)
					item.Item.BackColor = color;
				_logListView.EndUpdate();
			}
		}


		public void ClearAll()
		{
			ClearLogMessages();

			_loggerTreeView.BeginUpdate();

			foreach (KeyValuePair<string, LoggerItem> kvp in Loggers)
				kvp.Value.ClearAll();

			Node.Nodes.Clear();
			Loggers.Clear();

			_loggerTreeView.EndUpdate();
		}

		public void ClearLogMessages()
		{
			_logListView.BeginUpdate();
			
			if (Group != null)
				Group.Items.Clear();
			_logListView.Items.Clear();
			LogMessages.Clear();

			foreach (KeyValuePair<string, LoggerItem> kvp in Loggers)
				kvp.Value.ClearLogMessages();

			_logListView.EndUpdate();
		}

		internal void UpdateLogLevel()
		{
			_logListView.BeginUpdate();

			for (int i = 0; i < LogMessages.Count; i++)
			{
				LogMessageItem item = LogMessages[i];
				EnableLogMessage(item, item.IsLevelInRange());
			}

			foreach (KeyValuePair<string, LoggerItem> kvp in Loggers)
				kvp.Value.UpdateLogLevel();

			_logListView.EndUpdate();
		}

		private void EnableLogMessage(LogMessageItem item, bool enable)
		{
			if ((item.Enabled && enable) || (!item.Enabled && !enable))
				return;

			if (enable)
				EnableLogMessage(item);
			else
				DisableLogMessage(item);
		}

		private void EnableLogMessage(LogMessageItem item)
		{
			if (item.Enabled)
				return;

			InsertLogMessageByDate(_logListView.Items, item);

			if (Group != null)
				InsertLogMessageByDate(Group.Items, item);
		}

		private void InsertLogMessageByDate(ListView.ListViewItemCollection items, LogMessageItem refItem)
		{
			// Get previous item
			LogMessageItem prevItem = refItem;
			while ((prevItem != null) && !prevItem.Enabled)
				prevItem = prevItem.Previous;

			int pos = 0;
			if ((prevItem != null) && (prevItem.Item != null))
				pos = items.IndexOf(prevItem.Item) + 1;

			items.Insert(pos, refItem.Item);

			// Mark the item as enabled
			refItem.Enabled = true;
		}

		private void DisableLogMessage(LogMessageItem item)
		{
			if (!item.Enabled)
				return;

			if (Group != null)
				Group.Items.Remove(item.Item);

			_logListView.Items.Remove(item.Item);

			// Mark the item as disabled
			item.Enabled = false;
		}

		internal LoggerItem GetOrCreateLogger(string loggerPath)
		{
			if (String.IsNullOrEmpty(loggerPath))
				return null;

			// Extract Logger Name
			string currentLoggerName = loggerPath;
			string remainingLoggerPath = String.Empty;
			int pos = loggerPath.IndexOf(LoggerSeparator);
			if (pos > 0)
			{
				currentLoggerName = loggerPath.Substring(0, pos);
				remainingLoggerPath = loggerPath.Substring(pos + 1);
			}

			// Check if the Logger is in the Child Collection
			LoggerItem logger = null;
			if (!Loggers.TryGetValue(currentLoggerName, out logger))
			{
				// Not found here, needs to be created
				string childLoggerPath =
					(String.IsNullOrEmpty(FullName) ? "" : FullName + LoggerSeparator) + currentLoggerName;
				logger = LoggerItem.CreateLoggerItem(currentLoggerName, childLoggerPath, this);
			}

			// Continue?
			if (!String.IsNullOrEmpty(remainingLoggerPath))
				logger = logger.GetOrCreateLogger(remainingLoggerPath);

			return logger;
		}

		internal void AddLogMessage(LogMessage logMsg)
		{
			LogMessageItem item = new LogMessageItem(this, logMsg);
			item.Enabled = Enabled;
			LogMessages.Add(item);


			// We may remove and add many items, disable the drawing one moment
			_logListView.BeginUpdate();

			// Limit the number of displayed messages if necessary
			if (UserSettings.Instance.MessageCycleCount > 0)
				RemoveExtraLogMessages(UserSettings.Instance.MessageCycleCount);

			// Message
			if (Enabled)
			{
				// Set Previous item
				if (_logListView.Items.Count > 0)
					item.Previous = 
						_logListView.Items[_logListView.Items.Count - 1].Tag as LogMessageItem;

				// Add it to the main list
				_logListView.Items.Add(item.Item);

				// Add to the corresponding if necessary
				if (UserSettings.Instance.GroupLogMessages && (Group != null))
					Group.Items.Add(item.Item);

				// Force the item to be visible if necessary
				if (UserSettings.Instance.AutoScrollToLastLog)
					item.Item.EnsureVisible();
			}

			if (!item.IsLevelInRange())
				DisableLogMessage(item);

			// Done!
			_logListView.EndUpdate();
		}

		private void RemoveExtraLogMessages(int count)
		{
			// TODO: Buggy with Grouping....!!!

			int idx = 0;
			while (LogMessages.Count > count)
			{
				LogMessageItem item = LogMessages[idx];

				if (!item.Enabled)
				{
					count++;
					idx++;
				}

				RemoveLogMessage(item);
			}
		}

		private void RemoveLogMessage(LogMessageItem item)
		{
			LogMessages.Remove(item);

			if (Group != null)
				Group.Items.Remove(item.Item);

			_logListView.Items.Remove(item.Item);
		}
	}


	/// <summary>
	/// Describes a Log Message.
	/// TODO: Make it disposable to dereference Item?
	/// </summary>
	public class LogMessageItem
	{
		/// <summary>
		/// Logger Item Parent.
		/// </summary>
		public LoggerItem Parent;

		/// <summary>
		/// The item before this one, allow to retrieve the order of arrival (time is not reliable here).
		/// The previous item is not necessary a sibling in the logger tree, only in the message list view.
		/// </summary>
		public LogMessageItem Previous = null;

		/// <summary>
		/// The associated List View Item.
		/// </summary>
		public ListViewItem Item;

		/// <summary>
		/// Log Message.
		/// </summary>
		public LogMessage Message;

		/// <summary>
		/// Indicates if this Log Message Item is enable.
		/// When disabled the List View Item is not in the Log List View.
		/// </summary>
		public bool Enabled = true;


		public LogMessageItem(LoggerItem parent, LogMessage logMsg)
		{
			Parent = parent;
			Message = logMsg;

			// Create List View Item
			Item = new ListViewItem(logMsg.TimeStamp.ToString());
            Item.SubItems.Add(logMsg.Level.Name);
			Item.SubItems.Add(Parent.Name);
			Item.SubItems.Add(logMsg.ThreadName);
			Item.SubItems.Add(logMsg.Message);
			Item.ToolTipText = logMsg.Message;
            Item.ForeColor = logMsg.Level.Color;
			Item.Tag = this;
		}

		internal bool IsLevelInRange()
		{
			return (Message.Level.RangeMax >= UserSettings.Instance.LogLevelInfo.RangeMax);
		}
	}



	public class LogManager
	{
		private static LogManager _instance = null;

		private LoggerItem _rootLoggerItem = null;
		private Dictionary<string, LoggerItem> _fullPathLoggers = null;


		private LogManager()
		{
		}

		internal static LogManager Instance
		{
			get
			{
				if (_instance == null)
					_instance = new LogManager();
				return _instance;
			}
		}

		internal void Initialize(TreeView loggerTreeView, ListView logListView)
		{
			// Root Logger
			_rootLoggerItem = LoggerItem.CreateRootLoggerItem("Root", loggerTreeView, logListView);

			// Quick Access Logger Collection
			_fullPathLoggers = new Dictionary<string, LoggerItem>();

		}



		internal void ClearAll()
		{
			ClearLogMessages();

			_rootLoggerItem.ClearAll();
			_fullPathLoggers.Clear();
		}

		internal void ClearLogMessages()
		{
			_rootLoggerItem.ClearLogMessages();
		}


		internal void ProcessLogMessage(LogMessage logMsg)
		{
			// Check 1st in the global LoggerPath/Logger dictionary
			LoggerItem logger = null;
			if (!_fullPathLoggers.TryGetValue(logMsg.LoggerName, out logger))
			{
				// Not found, create one
				logger = _rootLoggerItem.GetOrCreateLogger(logMsg.LoggerName);
			}
			if (logger == null)
				throw new Exception("No Logger for this Log Message.");

			logger.AddLogMessage(logMsg);
		}


		internal void UpdateLogLevel()
		{
			if (_rootLoggerItem == null)
				return;
			_rootLoggerItem.UpdateLogLevel();
		}
	}
}
