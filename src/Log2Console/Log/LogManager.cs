using System;
using System.Collections.Generic;
using System.Windows.Forms;


namespace Log2Console.Log
{
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

        public void DeactivateLogger()
        {
            RootLoggerItem.Enabled = false;
        }

        public void ProcessLogMessage(LogMessage logMsg)
        {
            // Check 1st in the global LoggerPath/Logger dictionary
            LoggerItem logger;
            logMsg.CheckNull();

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
