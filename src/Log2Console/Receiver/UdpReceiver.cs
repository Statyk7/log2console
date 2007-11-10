using System;
using System.ComponentModel;
using System.Net;
using System.Net.Sockets;
using System.Threading;

using Log2Console.Log;


namespace Log2Console.Receiver
{
    [Serializable]
    public class UdpReceiver : BaseReceiver
    {
        [NonSerialized]
        private Thread _worker = null;
        [NonSerialized]
        private UdpClient _udpClient = null;
        [NonSerialized]
        private IPEndPoint _remoteEndPoint = null;

		private int _port = 7071;


        [Category("Configuration")]
		[DisplayName("UDP Port Number")]
		public int Port
		{
			get { return _port; }
			set { _port = value; }
		}


        #region IReceiver Members

        [Browsable(false)]
        public override string SampleClientConfig
        {
            get
            {
                return
                    "Configuration for log4net:" + Environment.NewLine +
                    "<appender name=\"UdpAppender\" type=\"log4net.Appender.UdpAppender\">" + Environment.NewLine +
                    "    <remoteAddress value=\"localhost\" />" + Environment.NewLine +
                    "    <remotePort value=\"7071\" />" + Environment.NewLine +
                    "    <layout type=\"log4net.Layout.XmlLayoutSchemaLog4j\" />" + Environment.NewLine +
                    "</appender>";
            }
        }

        public override void Initialize()
        {
            if ((_worker != null) && _worker.IsAlive)
                return;

            // Init connexion here, before starting the thread, to know the status now
            _remoteEndPoint = new IPEndPoint(IPAddress.Any, 0);
            _udpClient = new UdpClient(_port);

            // We need a working thread
            _worker = new Thread(Start);
            _worker.IsBackground = true;
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

                    LogMessage logMsg = ReceiverUtils.ParseLog4JXmlLogEvent(loggingEvent, "UdpLogger");
                    _notifiable.Notify(logMsg);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    return;
                }
            }
        }

    }
}
