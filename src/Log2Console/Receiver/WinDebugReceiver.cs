using System;
using System.ComponentModel;
using System.Diagnostics;

using CatchOutputDbg;

using Log2Console.Log;


namespace Log2Console.Receiver
{
    [Serializable]
    [DisplayName("WinDebug (OutputDebugString)")]
    public class WinDebugReceiver : BaseReceiver
    {
        #region Overrides of BaseReceiver

        [Browsable(false)]
        public override string SampleClientConfig
        {
            get { return "N/A"; }
        }

        public override void Initialize()
        {
            DebugMonitor.OnOutputDebugString += DebugMonitor_OnOutputDebugString;
            DebugMonitor.Start();
        }

        public override void Terminate()
        {
            DebugMonitor.OnOutputDebugString -= DebugMonitor_OnOutputDebugString;
            DebugMonitor.Stop();
        }

        #endregion


        void DebugMonitor_OnOutputDebugString(int pid, string text)
        {
            // Trim ending newline (if any) 
            if (text.EndsWith(Environment.NewLine))
                text = text.Substring(0, text.Length - Environment.NewLine.Length);

            // Replace dots by "middle dots" to preserve Logger namespace
            string processName = GetProcessName(pid);
            processName = processName.Replace('.', '·');

            LogMessage logMsg = new LogMessage();
            logMsg.Message = text;
            logMsg.LoggerName = processName;
            logMsg.LoggerName = String.Format("{0}.{1}", processName, pid);
            logMsg.Level = LogLevels.Instance[LogLevel.Debug];
            logMsg.ThreadName = pid.ToString();
            logMsg.TimeStamp = DateTime.Now;
            Notifiable.Notify(logMsg);
        }

        private static string GetProcessName(int pid)
        {
            if (pid == -1)
                return Process.GetCurrentProcess().ProcessName;
            try
            {
                return Process.GetProcessById(pid).ProcessName;
            }
            catch
            {
                return "<exited>";
            }
        }
    }
}
