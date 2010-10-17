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
        public LoggerItem Parent;
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
        public ILoggerView LoggerView;
        /// <summary>
        /// The optional associated List View Group.
        /// </summary>
        public ListViewGroup Group;
        /// <summary>
        /// A reference to the Log ListView associated to this Logger.
        /// </summary>
        private ListView _logListView;

        /// <summary>
        /// Short Name of this Logger (used as the node name).
        /// </summary>
        private string _name = String.Empty;
        /// <summary>
        /// Full Name (or "Path") of this Logger.
        /// </summary>
        public string FullName = String.Empty;
        /// <summary>
        /// When set the Logger and its Messages are displayed.
        /// </summary>
        private bool _enabled = true;

        private bool _highlight;
        private bool _highlightLogMessages;

        private string _searchedText;
        private bool _hasSearchedText;
        

        private LoggerItem()
        {
        }

        public static LoggerItem CreateRootLoggerItem(string name, ILoggerView loggerView, ListView logListView)
        {
            LoggerItem logger = new LoggerItem();
            logger.Name = name;
            logger._logListView = logListView;

            // Tree Node
            logger.LoggerView = loggerView.AddNew(name, logger);

            return logger;
        }

        private static LoggerItem CreateLoggerItem(string name, string fullName, LoggerItem parent)
        {
            if (parent == null)
                throw new ArgumentNullException();

            // Creating the logger item.
            LoggerItem logger = new LoggerItem();
            logger.Name = name;
            logger.FullName = fullName;
            logger.Parent = parent;


            logger._logListView = logger.Parent._logListView;

            // Adding the logger as a child of the parent logger.
            logger.Parent.Loggers.Add(name, logger);

            // Creating a child logger view and saving it in the new logger.
            logger.LoggerView = parent.LoggerView.AddNew(name, logger);
            
            if (UserSettings.Instance.RecursivlyEnableLoggers)
            {
                logger._enabled = parent.Enabled;
                logger.LoggerView.Enabled = parent.LoggerView.Enabled;
            }

            // Group
            if (UserSettings.Instance.GroupLogMessages)
            {
                logger.Group = new ListViewGroup(fullName);
                logger._logListView.Groups.Add(logger.Group);
            }

            return logger;
        }



        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;

                if (LoggerView != null)
                    LoggerView.Text = _name;
            }
        }

        internal bool Enabled
        {
            get { return _enabled; }
            set
            {
                if (_enabled == value)
                    return;

                _enabled = value;
                LoggerView.Enabled = value;

                // Now enable all child loggers if the settings are set to 
                // recursivly enable/disable chid loggers.
                if (UserSettings.Instance.RecursivlyEnableLoggers)
                {
                    foreach (LoggerItem child in Loggers.Values)
                    {
                        child.Enabled = value;
                    }
                }

                if (LogMessages.Count == 0)
                    return;

                // Update List View
                _logListView.BeginUpdate();

                foreach (LogMessageItem item in LogMessages)
                {
                    EnableLogMessage(item, _enabled && IsItemToBeEnabled(item));
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
                LoggerView.Highlight = value;
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


                _logListView.BeginUpdate();

                foreach (LogMessageItem item in LogMessages)
                    item.Highlight(value);

                _logListView.EndUpdate();
            }
        }

        public void Remove()
        {
            if (Parent == null)
            {
                // If root, clear all
                ClearAll();
                return;
            }

            ClearLogMessages();

            Parent.Loggers.Remove(Name);
            Parent.LoggerView.Remove(Name);
        }

        public void ClearAll()
        {
            ClearAllLogMessages();

            foreach (KeyValuePair<string, LoggerItem> kvp in Loggers)
                kvp.Value.ClearAll();

            LoggerView.Clear();
            Loggers.Clear();
        }

        public void ClearAllLogMessages()
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

        public void ClearLogMessages()
        {
            _logListView.BeginUpdate();
            
            if (Group != null)
                Group.Items.Clear();

            foreach (LogMessageItem item in LogMessages)
                _logListView.Items.Remove(item.Item);
            LogMessages.Clear();

            foreach (KeyValuePair<string, LoggerItem> kvp in Loggers)
                kvp.Value.ClearLogMessages();

            _logListView.EndUpdate();
        }

        internal void UpdateLogLevel()
        {
            _logListView.BeginUpdate();

            foreach (LogMessageItem item in LogMessages)
            {
                EnableLogMessage(item, IsItemToBeEnabled(item));
            }

            foreach (KeyValuePair<string, LoggerItem> kvp in Loggers)
            {
                kvp.Value.UpdateLogLevel();
            }

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
            LoggerItem logger;
            if (!Loggers.TryGetValue(currentLoggerName, out logger))
            {
                // Not found here, needs to be created
                string childLoggerPath =
                    (String.IsNullOrEmpty(FullName) ? "" : FullName + LoggerSeparator) + currentLoggerName;
                logger = CreateLoggerItem(currentLoggerName, childLoggerPath, this);
            }

            // Continue?
            if (!String.IsNullOrEmpty(remainingLoggerPath))
                logger = logger.GetOrCreateLogger(remainingLoggerPath);

            return logger;
        }

        internal LogMessageItem AddLogMessage(LogMessage logMsg)
        {
            LogMessageItem item = new LogMessageItem(this, logMsg);
            item.Enabled = Enabled;
            LogMessages.Add(item);


            // We may remove and add many items, disable the drawing one moment
            _logListView.BeginUpdate();

            // Limit the number of displayed messages if necessary
            if (UserSettings.Instance.MessageCycleCount > 0)
                RemoveExtraLogMessages(UserSettings.Instance.MessageCycleCount);

            // Set Previous item
            if (_logListView.Items.Count > 0)
                item.Previous = 
                    _logListView.Items[_logListView.Items.Count - 1].Tag as LogMessageItem;

            // Message
            if (Enabled)
            {
                // Add it to the main list
                _logListView.Items.Add(item.Item);

                // Add to the corresponding if necessary
                if (UserSettings.Instance.GroupLogMessages && (Group != null))
                    Group.Items.Add(item.Item);

                // Force the item to be visible if necessary
                if (UserSettings.Instance.AutoScrollToLastLog)
                    item.Item.EnsureVisible();
            }

            // Hide the item is is not in range or doesn't match the current text search if any
            if (!IsItemToBeEnabled(item))
                DisableLogMessage(item);

            // Done!
            _logListView.EndUpdate();

            return item;
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

        internal void SearchText(string str)
        {
            _logListView.BeginUpdate();

            DoSearch(str);

            _logListView.EndUpdate();
        }

        private void DoSearch(string str)
        {
            _hasSearchedText = !String.IsNullOrEmpty(str);
            _searchedText = str;

            // Skip all log messages if logger is disabled
            if (Enabled)
            {
                foreach (LogMessageItem item in LogMessages)
                {
                    EnableLogMessage(item, IsItemToBeEnabled(item));
                }
            }

            // Iterate call
            foreach (KeyValuePair<string, LoggerItem> kvp in Loggers)
            {
                kvp.Value.DoSearch(_searchedText);
            }
        }

        internal bool IsItemToBeEnabled(LogMessageItem item)
        {
            return (item.IsLevelInRange() && (!_hasSearchedText || item.HasSearchedText(_searchedText)));
        }

        public override string ToString()
        {
            return Name;
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
        public LogMessageItem Previous;

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

            string parentName = Parent.Name;
            if ((Parent.Name == logMsg.ThreadName) && (Parent.Parent != null))
                parentName = Parent.Parent.Name;

            // Create List View Item
            Item = new ListViewItem(logMsg.TimeStamp.ToString(UserSettings.Instance.TimeStampFormatString));
            Item.SubItems.Add(logMsg.Level.Name);
            Item.SubItems.Add(parentName);
            Item.SubItems.Add(logMsg.ThreadName);

            string msg = logMsg.Message.Replace("\r\n", " ");
            msg = msg.Replace("\n", " ");
            Item.SubItems.Add(msg);
            Item.ToolTipText = msg;

            Item.ForeColor = logMsg.Level.Color;
            Item.Tag = this;
        }

        internal void Highlight(bool state)
        {
            Item.BackColor = state ? Color.LightBlue : Color.Transparent;
        }

        internal bool IsLevelInRange()
        {
            return (Message.Level.RangeMax >= UserSettings.Instance.LogLevelInfo.RangeMax);
        }

        internal void HighlightSearchedText(bool hasText, string str)
        {
            if (hasText && HasSearchedText(str))
                Item.BackColor = Color.LightYellow;
            else
                Item.BackColor = Color.Transparent;
        }
        
        internal bool HasSearchedText(string str)
        {
            return (Message.Message.IndexOf(str, StringComparison.InvariantCultureIgnoreCase) >= 0);
        }
    }



    public class LogManager : ILogManager
    {
        private static LogManager _instance;

        private LoggerItem _rootLoggerItem;
        private Dictionary<string, LoggerItem> _fullPathLoggers;


        private LogManager()
        {
        }

        internal static ILogManager Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new LogManager();
                return _instance;
            }
        }

        internal LoggerItem RootLoggerItem
        {
            get { return _rootLoggerItem; }
            set { _rootLoggerItem = value; }
        }

        public void Initialize(ILoggerView loggerView, ListView logListView)
        {
            // Root Logger
            _rootLoggerItem = LoggerItem.CreateRootLoggerItem("Root", loggerView, logListView);

            // Quick Access Logger Collection
            _fullPathLoggers = new Dictionary<string, LoggerItem>();
        }

        public void ClearAll()
        {
            ClearLogMessages();

            RootLoggerItem.ClearAll();
            _fullPathLoggers.Clear();
        }

        public void ClearLogMessages()
        {
            RootLoggerItem.ClearAllLogMessages();
        }

        public void ProcessLogMessage(LogMessage logMsg)
        {
            // Check 1st in the global LoggerPath/Logger dictionary
            LoggerItem logger;
            if (!_fullPathLoggers.TryGetValue(logMsg.LoggerName, out logger))
            {
                // Not found, create one
                logger = RootLoggerItem.GetOrCreateLogger(logMsg.LoggerName);
            }
            if (logger == null)
                throw new Exception("No Logger for this Log Message.");

            logger.AddLogMessage(logMsg);
        }


        public void SearchText(string str)
        {
            _rootLoggerItem.SearchText(str);
        }


        public void UpdateLogLevel()
        {
            if (RootLoggerItem == null)
                return;

            RootLoggerItem.UpdateLogLevel();
        }

        public void SetRootLoggerName(string name)
        {
            RootLoggerItem.Name = name;
        }
    }
}
