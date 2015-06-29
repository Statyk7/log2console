using Log2Console.Settings;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

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
        public Dictionary<string, LoggerItem> Loggers = new Dictionary<string, LoggerItem>();
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

            int index = 0;
            if (_logListView.Items.Count > 0)
            {
                for (index = _logListView.Items.Count; index > 0; --index)
                {
                    item.Previous = _logListView.Items[index - 1].Tag as LogMessageItem;
                    if (item.Previous.Message.TimeStamp.Ticks <= item.Message.TimeStamp.Ticks)
                        break;
                }
            }

            // Message
            if (Enabled)
            {
                // Add it to the main list
                _logListView.Items.Insert(index, item.Item);

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
}
