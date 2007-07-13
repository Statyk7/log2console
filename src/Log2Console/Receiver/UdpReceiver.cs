using System;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Xml;

using Log2Console.Log;


namespace Log2Console.Receiver
{
    public class UdpReceiver : BaseReceiver
    {
        static readonly DateTime s1970 = new DateTime(1970, 1, 1);

        private Thread _worker = null;
        private UdpClient _udpClient = null;
        private IPEndPoint _remoteEndPoint = null;

		private int _port = 7071;


		[DisplayName("UDP Port Number")]
		public int Port
		{
			get { return _port; }
			set { _port = value; }
		}


        #region IReceiver Members

        public override void Initialize()
        {
            if ((_worker != null) && _worker.IsAlive)
                return;

            // Init connexion here, before starting the thread, to know the status now
            _remoteEndPoint = new IPEndPoint(IPAddress.Any, 0);
            _udpClient = new UdpClient(_port);

            // We need a working thread
            _worker = new Thread(Start);
            _worker.Start();
        }

        public override void Terminate()
        {
            if (_udpClient != null)
            {
                _udpClient.Close();
                _udpClient = null;

                _remoteEndPoint = null;
            }

            if ((_worker != null) && _worker.IsAlive)
                _worker.Abort();
            _worker = null;
        }

        #endregion


        private void Start()
        {
            while ((_udpClient != null) && (_remoteEndPoint != null))
            {
                try
                {
                    byte[] buffer = _udpClient.Receive(ref _remoteEndPoint);
                    string loggingEvent = System.Text.Encoding.ASCII.GetString(buffer);

                    Console.WriteLine(loggingEvent);

                    if (_notifiable == null)
                        continue;

                    LogMessage logMsg = CreateLogMessage(loggingEvent);
                    _notifiable.Notify(logMsg);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    return;
                }
            }
        }


        /// <summary>
        /// Here we expect the log event to use the log4j schema.
        /// Sample:
        ///     <log4j:event logger="Statyk7.Another.Name.DummyManager" timestamp="1184286222308" level="ERROR" thread="1">
        ///         <log4j:message>This is an Message</log4j:message>
        ///         <log4j:properties>
        ///             <log4j:data name="log4jmachinename" value="remserver" />
        ///             <log4j:data name="log4net:HostName" value="remserver" />
        ///             <log4j:data name="log4net:UserName" value="REMSERVER\Statyk7" />
        ///             <log4j:data name="log4japp" value="Test.exe" />
        ///         </log4j:properties>
        ///     </log4j:event>
        /// </summary>
        /// 
        /// Partial implementation taken from: http://geekswithblogs.net/kobush/archive/2006/04/20/75717.aspx
        /// 
        protected static LogMessage CreateLogMessage(string logEvent)
        {
            LogMessage logMsg = new LogMessage();
            
            try
            {
                NameTable nt = new NameTable();
                XmlNamespaceManager nsmanager = new XmlNamespaceManager(nt);
                nsmanager.AddNamespace("log4j", "http://jakarta.apache.org/log4j/");
                XmlParserContext context =
                    new XmlParserContext(nt, nsmanager, "elem", XmlSpace.None, Encoding.ASCII);

                XmlTextReader reader = new XmlTextReader(logEvent, XmlNodeType.Element, context);
                reader.Read();
                reader.MoveToContent();

                logMsg.LoggerName = reader.GetAttribute("logger");
                logMsg.Level = LogLevels.Instance[reader.GetAttribute("level")];
                logMsg.ThreadName = reader.GetAttribute("thread");

                long timeStamp;
                if (long.TryParse(reader.GetAttribute("timestamp"), out timeStamp))
                    logMsg.TimeStamp = s1970.AddMilliseconds(timeStamp).ToLocalTime();

                int eventDepth = reader.Depth;
                reader.Read();
                while (reader.Depth > eventDepth)
                {
                    if (reader.MoveToContent() == XmlNodeType.Element)
                    {
                        switch (reader.Name)
                        {
                            case "log4j:message":
                                logMsg.Message = reader.ReadString();
                                break;
                            case "log4j:throwable":
                                reader.ReadString();
                                break;
                            case "log4j:locationInfo":
                                break;
                            case "log4j:properties":
                                reader.Read();
                                while (reader.MoveToContent() == XmlNodeType.Element
                                       && reader.Name == "log4j:data")
                                {
                                    string name = reader.GetAttribute("name");
                                    string value = reader.GetAttribute("value");
                                    logMsg.Properties[name] = value;
                                    reader.Read();
                                }
                                break;
                        }
                    }
                    reader.Read();
                }
            }
            catch (Exception ex)
            {
                logMsg.LoggerName = "UdpReceiverLogger";
                logMsg.ThreadName = String.Empty;
                logMsg.Message = logEvent;
                logMsg.TimeStamp = DateTime.Now;
                logMsg.Level = LogLevels.Instance[LogLevel.Info];
            }

            return logMsg;
        }
    }
}
