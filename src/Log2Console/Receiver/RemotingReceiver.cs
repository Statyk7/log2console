using System;
using System.ComponentModel;
using System.Collections;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Runtime.Serialization.Formatters;

using log4net.Appender;
using log4net.Core;

using Log2Console.Log;


namespace Log2Console.Receiver
{
    public class RemotingReceiver : BaseReceiver, RemotingAppender.IRemoteLoggingSink
    {
		private const string RemotingReceiverChannelName = "RemotingReceiverChannel";

		private string _sinkName = "LoggingSink";
		private int _port = 7070;

		private IChannel _channel = null;


		[DisplayName("Remote Sink Name")]
		public string SinkName
		{
			get { return _sinkName; }
			set { _sinkName = value; }
		}

		[DisplayName("Remote TCP Port Number")]
		public int Port
		{
			get { return _port; }
			set { _port = value; }
		}


        #region IReceiver Members

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


        protected static LogMessage CreateLogMessage(LoggingEvent logEvent)
        {
            LogMessage logMsg = new LogMessage();

            logMsg.LoggerName = logEvent.LoggerName;
            logMsg.ThreadName = logEvent.ThreadName;
            logMsg.Message = logEvent.RenderedMessage;
            logMsg.TimeStamp = logEvent.TimeStamp;
            logMsg.Level = LogUtils.GetLogLevelInfo(logEvent.Level.Value);

            return logMsg;
        }
    }
}
