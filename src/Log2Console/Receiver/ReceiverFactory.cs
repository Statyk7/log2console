using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Text;
using System.Xml;

using Log2Console.Log;


namespace Log2Console.Receiver
{
    public static class ReceiverUtils
    {
        static readonly DateTime s1970 = new DateTime(1970, 1, 1);


        public static string GetTypeDescription(Type type)
        {
            object[] attributes = type.GetCustomAttributes(true);
            foreach (object attribute in attributes)
            {
                DisplayNameAttribute da = attribute as DisplayNameAttribute;
                if (da != null)
                    return da.DisplayName;
            }

            return type.ToString();
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
        /// Implementation inspired from: http://geekswithblogs.net/kobush/archive/2006/04/20/75717.aspx
        /// 
        public static LogMessage ParseLog4JXmlLogEvent(string logEvent, string defaultLogger)
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
                if ((reader.MoveToContent() != XmlNodeType.Element) || (reader.Name != "log4j:event"))
                    throw new Exception("The Log Event is not a valid log4j Xml block.");

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
                                logMsg.Message += Environment.NewLine + reader.ReadString();
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
            catch (Exception /*ex*/)
            {
                // Create a simple log message with some default values
                logMsg.LoggerName = defaultLogger;
                logMsg.ThreadName = "NA";
                logMsg.Message = logEvent;
                logMsg.TimeStamp = DateTime.Now;
                logMsg.Level = LogLevels.Instance[LogLevel.Info];
            }

            return logMsg;
        }
    }

	public class ReceiverFactory
	{
	    public class ReceiverInfo
        {
            public string Name;
            public Type Type;

            public override string ToString()
            {
                return Name;
            }
        }

		private static ReceiverFactory _instance;

        private readonly Dictionary<string, ReceiverInfo> _receiverTypes = new Dictionary<string, ReceiverInfo>();


	    private static readonly string ReceiverInterfaceName = typeof(IReceiver).FullName;


		private ReceiverFactory()
		{
            // Get all the possible receivers by enumerating all the types implementing the interface
		    Assembly assembly = Assembly.GetAssembly(typeof(IReceiver));
            Type[] types = assembly.GetTypes();
            foreach (Type type in types)
            {
                // Skip abstract types
                if (type.IsAbstract)
                    continue;

                Type[] findInterfaces = type.FindInterfaces((typeObj, o) => (typeObj.ToString() == ReceiverInterfaceName), null);
                if (findInterfaces.Length < 1)
                    continue;

                AddReceiver(type);
            }
		}

        private void AddReceiver(Type type)
        {
            ReceiverInfo info = new ReceiverInfo();
            info.Name = ReceiverUtils.GetTypeDescription(type);
            info.Type = type;

			_receiverTypes.Add(type.FullName, info);
        }

		public static ReceiverFactory Instance
		{
			get { return _instance ?? (_instance = new ReceiverFactory()); }
		}

        public Dictionary<string, ReceiverInfo> ReceiverTypes
		{
			get { return _receiverTypes; }
		}


		public IReceiver Create(string typeStr)
		{
			IReceiver receiver = null;

            ReceiverInfo info;
			if (_receiverTypes.TryGetValue(typeStr, out info))
			{
			    receiver = Activator.CreateInstance(info.Type) as IReceiver;
			}

			return receiver;
		}
	}
}
