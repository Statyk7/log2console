using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml;

using Log2Console.Log;

namespace Log2Console.Receiver
{
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
      var info = new ReceiverInfo { Name = ReceiverUtils.GetTypeDescription(type), Type = type };
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
