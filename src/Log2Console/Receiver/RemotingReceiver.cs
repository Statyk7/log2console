using System;
using System.ComponentModel;
using System.Collections;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters;

using log4net.Appender;
using log4net.Core;

using Log2Console.Log;


namespace Log2Console.Receiver
{
    [Serializable]
    public class RemotingReceiver : BaseReceiver, RemotingAppender.IRemoteLoggingSink, ISerializable
    {
        private const string RemotingReceiverChannelName = "RemotingReceiverChannel";

        [NonSerialized]
        private IChannel _channel = null;

        private string _sinkName = "LoggingSink";
        private int _port = 7070;
		private bool _appendHostNameToLogger = true;


        [Category("Configuration")]
        [DisplayName("Remote Sink Name")]
        public string SinkName
        {
            get { return _sinkName; }
            set { _sinkName = value; }
        }

        [Category("Configuration")]
        [DisplayName("Remote TCP Port Number")]
        public int Port
        {
            get { return _port; }
            set { _port = value; }
		}
		
		[Category("Configuration")]
		[DisplayName("Append Host Name to Logger")]
		[Description("Append the remote Host Name to the Logger Name.")]
		public bool AppendHostNameToLogger
		{
			get { return _appendHostNameToLogger; }
			set { _appendHostNameToLogger = value; }
		}


        /// <summary>
        /// Default ctor
        /// </summary>
        public RemotingReceiver() { }

        #region ISerializable Members

        /// <summary>
        /// Constructor for Serialization
        /// N.B: Explicit implementation of ISerializable to mask SecurityIdentity Property of mother class
        /// </summary>
        public RemotingReceiver(SerializationInfo info, StreamingContext context)
        {
            _sinkName = info.GetString("SinkName");
            _port = info.GetInt32("Port");
        }

        /// <summary>
        /// ISerializable method override for deserialization
        /// N.B: Explicit implementation of ISerializable to mask SecurityIdentity Property of mother class
        /// </summary>
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("SinkName", _sinkName);
            info.AddValue("Port", _port);
        }

        #endregion


        #region IReceiver Members

        [Browsable(false)]
        public override string SampleClientConfig
        {
            get
            {
                return
                    "Configuration for log4net:" + Environment.NewLine +
                    "<appender name=\"RemotingAppender\" type=\"log4net.Appender.RemotingAppender\" >" + Environment.NewLine +
                    "    <!--The remoting URL to the remoting server object-->" + Environment.NewLine +
                    "    <sink value=\"tcp://localhost:7070/LoggingSink\" />" + Environment.NewLine +
                    "    <!--Send all events, do not discard events when the buffer is full-->" + Environment.NewLine +
                    "    <lossy value=\"false\" />" + Environment.NewLine +
                    "    <!--The number of events to buffer before sending-->" + Environment.NewLine +
                    "    <bufferSize value=\"5\" />" + Environment.NewLine +
                    "</appender>";
            }
        }

        public override void Initialize()
		{
			// Channel already open?
			_channel = ChannelServices.GetChannel(RemotingReceiverChannelName);


			if (_channel == null)
			{
				// Allow clients to receive complete Remoting exception information
				if (RemotingConfiguration.CustomErrorsEnabled(true))
					RemotingConfiguration.CustomErrorsMode = CustomErrorsModes.Off;

				// Create TCP Channel
				try
				{
					BinaryClientFormatterSinkProvider clientProvider = null;
					BinaryServerFormatterSinkProvider serverProvider =
						new BinaryServerFormatterSinkProvider();
					serverProvider.TypeFilterLevel = TypeFilterLevel.Full;

					IDictionary props = new Hashtable();
					props["port"] = _port.ToString();
					props["name"] = RemotingReceiverChannelName;
					props["typeFilterLevel"] = TypeFilterLevel.Full;
					_channel = new TcpChannel(props, clientProvider, serverProvider);

					ChannelServices.RegisterChannel(_channel, false);
				}
				catch (Exception ex)
				{
					throw new Exception("Remoting TCP Channel Initialization failed", ex);
				}
			}

			Type serverType = RemotingServices.GetServerTypeForUri(_sinkName);
			if ((serverType == null) || (serverType != typeof(RemotingAppender.IRemoteLoggingSink)))
			{
				// Marshal Receiver
				try
				{
					RemotingServices.Marshal(this, _sinkName, typeof(RemotingAppender.IRemoteLoggingSink));
				}
				catch (Exception ex)
				{
					throw new Exception("Remoting Marshal failed", ex);
				}
			}
		}

        public override void Terminate()
		{
			if (_channel != null)
				ChannelServices.UnregisterChannel(_channel);
			_channel = null;
		}

        #endregion


        #region Override implementation of MarshalByRefObject

        /// <summary>
        /// Obtains a lifetime service object to control the lifetime 
        /// policy for this instance.
        /// </summary>
        /// <returns><c>null</c> to indicate that this instance should live forever.</returns>
        /// <remarks>
        /// <para>
        /// Obtains a lifetime service object to control the lifetime 
        /// policy for this instance. This object should live forever
        /// therefore this implementation returns <c>null</c>.
        /// </para>
        /// </remarks>
        public override object InitializeLifetimeService()
        {
            return null;
        }

        #endregion Override implementation of MarshalByRefObject


        #region Implementation of IRemoteLoggingSink

        public void LogEvents(LoggingEvent[] events)
        {
			if ((events == null) || (events.Length == 0) || (_notifiable == null))
                return;

			LogMessage[] logMsgs = new LogMessage[events.Length];
			for (int i = 0; i < events.Length; i++)
				logMsgs[i] = CreateLogMessage(events[i]);

			_notifiable.Notify(logMsgs);
        }

        #endregion Implementation of IRemoteLoggingSink


        protected LogMessage CreateLogMessage(LoggingEvent logEvent)
        {
            LogMessage logMsg = new LogMessage();

			logMsg.LoggerName = _appendHostNameToLogger && logEvent.Properties.Contains(LoggingEvent.HostNameProperty)
        	                    	? String.Format("[Host: {0}].{1}", logEvent.Properties[LoggingEvent.HostNameProperty], logEvent.LoggerName)
        	                    	: logEvent.LoggerName;

        	logMsg.ThreadName = logEvent.ThreadName;
            logMsg.Message = logEvent.RenderedMessage;
            logMsg.TimeStamp = logEvent.TimeStamp;
            logMsg.Level = LogUtils.GetLogLevelInfo(logEvent.Level.Value);

            // Per LoggingEvent.ExceptionObject, the exception object is not serialized, but the exception 
            // text is available through LoggingEvent.GetExceptionString
            logMsg.ExceptionString = logEvent.GetExceptionString();

            // Copy properties as string
            foreach (DictionaryEntry entry in logEvent.Properties)
            {
                if ((entry.Key == null) || (entry.Value == null))
                    continue;

                logMsg.Properties.Add(entry.Key.ToString(), entry.Value.ToString());
            }

            return logMsg;
        }
    }
}
