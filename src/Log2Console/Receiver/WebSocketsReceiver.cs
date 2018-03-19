using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Log2Console.Log;


namespace Log2Console.Receiver
{
    [Serializable]
    [DisplayName("WebSockets")]
    public class WebSocketsReceiver : BaseReceiver
    {
        private string _serverUri = @"wss://localhost:443";
        private string _handshakeMsg = string.Empty;
        private int _bufferSize = 10000;

        [NonSerialized]
        private Thread _worker;

        [NonSerialized]
        private CancellationToken _cancellationToken;

        [NonSerialized]
        private ClientWebSocket _websocketClient;

        [NonSerialized]
        private byte[] _buffer;

        [NonSerialized]
        private StringBuilder _messageBuilder;


        [Category("Configuration")]
        [DisplayName("Server Host")]
        [DefaultValue("ws://localhost:80")]
        public string WebSocketServerUri
        {
            get { return _serverUri; }
            set
            {
                _serverUri = value;
                this.Connect();
            }
        }

        [Category("Configuration")]
        [DisplayName("Handshake Msg.")]
        [DefaultValue("")]
        public string WebsocketHandshakeMsg
        {
            get { return _handshakeMsg; }
            set
            {
                _handshakeMsg = value;
                this.Connect();
            }
        }

        [Category("Configuration")]
        [DisplayName("Receive Buffer Size")]
        public int BufferSize
        {
            get { return _bufferSize; }
            set
            {
                _bufferSize = value;
                this.Connect();
            }
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

            Connect();

            // We need a working thread
            _worker = new Thread(Start);
            _worker.IsBackground = true;
            _worker.Start();
        }

        private void Connect()
        {
            try
            {
                if (this._websocketClient != null)
                {
                    this.Disconnect();
                }

                this._buffer = new byte[this._bufferSize];
                this._messageBuilder = new StringBuilder();

                this._websocketClient = new ClientWebSocket();
                this._cancellationToken = new CancellationToken();
                _websocketClient
                    .ConnectAsync(new Uri(this._serverUri), this._cancellationToken)
                    .ContinueWith(WssAuthenticate, this._cancellationToken);
            }
            catch (Exception ex)
            {
                this._websocketClient = null;
                Console.WriteLine(ex);
            }
        }

        private void WssAuthenticate(Task obj)
        {
            if (!string.IsNullOrEmpty(_handshakeMsg))
            {
                ArraySegment<byte> buffer = new ArraySegment<byte>(Encoding.UTF8.GetBytes(_handshakeMsg));

                _websocketClient
                    .SendAsync(buffer, WebSocketMessageType.Text, true, this._cancellationToken)
                    .ContinueWith(AuthenticationComplete, this._cancellationToken);
            }
        }

        private void AuthenticationComplete(Task obj)
        {
            // ignore it
        }

        public override void Terminate()
        {
            Disconnect();

            if ((_worker != null) && _worker.IsAlive)
                _worker.Abort();
            _worker = null;
        }

        private void Disconnect()
        {
            try
            {
                if (this._websocketClient != null)
                {
                    this._websocketClient.Abort();
                    this._websocketClient.Dispose();
                    this._websocketClient = null;
                }
            }
            catch (Exception ex)
            {
                this._websocketClient = null;
                Console.WriteLine(ex);
            }
        }

        #endregion

        public void Clear()
        {
        }

        private void Start()
        {
            ArraySegment<byte> buffer = new ArraySegment<byte>(this._buffer);
            var lastState = this._websocketClient?.State;

            while (true)
            {
                try
                {
                    if (this._websocketClient != null && lastState != this._websocketClient.State)
                    {
                        this.NotifyWebSocketStateChange(this._websocketClient.State);
                    }

                    lastState = this._websocketClient?.State;

                    if (this._websocketClient == null 
                        || this._websocketClient.State != WebSocketState.Open 
                        || Notifiable == null)
                    {
                        Thread.Sleep(150); // don't let it throttle so badly
                        continue;
                    }

                    this._websocketClient.ReceiveAsync(buffer, this._cancellationToken)
                        .ContinueWith(OnBufferReceived, _cancellationToken)
                        .Wait(this._cancellationToken);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    return;
                }
            }
        }

        private void OnBufferReceived(Task<WebSocketReceiveResult> obj)
        {
            if (obj.IsCompleted)
            {
                var loggingEvent = Encoding.UTF8.GetString(this._buffer);
                this._messageBuilder.Append(loggingEvent);

                Console.WriteLine(loggingEvent);

                if (obj.Result.EndOfMessage)
                {
                    var logMsg = ReceiverUtils.ParseLog4JXmlLogEvent(loggingEvent, "wssLogger");
                    logMsg.Level = LogLevels.Instance[LogLevel.Debug];

                    var loggerName = this._serverUri.Replace("wss://", "wss-").Replace(":", "-").Replace(".", "-");
                    logMsg.RootLoggerName = loggerName;
                    logMsg.LoggerName = $"{loggerName}_{logMsg.LoggerName}";
                    Notifiable.Notify(logMsg);

                    this._messageBuilder.Clear();
                }
            }
        }

        private void NotifyWebSocketStateChange(WebSocketState state)
        {
            var logMsg = ReceiverUtils.ParseLog4JXmlLogEvent($"WebSocket state changed: {state}", "wssLogger");
            logMsg.Level = LogLevels.Instance[LogLevel.Info];

            var loggerName = this._serverUri.Replace("wss://", "wss-").Replace(":", "-").Replace(".", "-");
            logMsg.RootLoggerName = loggerName;
            logMsg.LoggerName = $"{loggerName}_{logMsg.LoggerName}";
            Notifiable.Notify(logMsg);
        }
    }
}
