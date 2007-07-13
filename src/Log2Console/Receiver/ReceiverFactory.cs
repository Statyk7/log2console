using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;


namespace Log2Console.Receiver
{
	public class ReceiverFactory
	{
		private static ReceiverFactory _instance = null;

		private Dictionary<string, Type> _receiverTypes = null;


		private ReceiverFactory()
		{
			_receiverTypes = new Dictionary<string, Type>();

			// TODO: Populate using reflection onto assembly

			// Remoting Receiver
			Type type = typeof(RemotingReceiver);
			_receiverTypes.Add(type.FullName, type);
            
            // UDP Receiver
			type = typeof(UdpReceiver);
			_receiverTypes.Add(type.FullName, type);

		}

		public static ReceiverFactory Instance
		{
			get {
				if (_instance == null)
					_instance = new ReceiverFactory();
				return _instance;
			}
		}

		public Dictionary<string, Type> ReceiverTypes
		{
			get { return _receiverTypes; }
		}


		public IReceiver Create(string typeStr)
		{
			IReceiver receiver = null;

			Type type = null;
			if (_receiverTypes.TryGetValue(typeStr, out type))
			{
				receiver = 
					Assembly.GetExecutingAssembly().CreateInstance(typeStr)
						as IReceiver;
			}

			return receiver;
		}
	}
}
