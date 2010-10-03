using System;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Log2Console.Receiver
{
  [Serializable]
  [DisplayName("TCP (IP v4 and v6)")]
  public class TcpReceiver : BaseReceiver
  {
    #region Port Property
    
    int _port = 4505;
    [Category("Configuration")]
    [DisplayName("TCP Port Number")]
    [DefaultValue(4505)]
    public int Port
    {
      get { return _port; }
      set { _port = value; }
    }

    #endregion

    #region IpV6 Property
    
    bool _ipv6;
    [Category("Configuration")]
    [DisplayName("Use IPv6 Addresses")]
    [DefaultValue(false)]
    public bool IpV6
    {
      get { return _ipv6; }
      set { _ipv6 = value; }
    }

    #endregion
    
    #region IReceiver Members

    [Browsable(false)]
    public override string SampleClientConfig
    {
      get
      {
        return
            "Configuration for NLog:" + Environment.NewLine +
            "<target name=\"TcpOutlet\" xsi:type=\"NLogViewer\" address=\"tcp://localhost:4505\"/>";
      }
    }

    [NonSerialized]
    Socket _socket;

    public override void Initialize()
    {
      if (_socket != null) return;

      _socket = new Socket(_ipv6 ? AddressFamily.InterNetworkV6 : AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
      _socket.ExclusiveAddressUse = true;
      _socket.Bind(new IPEndPoint(_ipv6 ? IPAddress.IPv6Any : IPAddress.Any, _port));
      _socket.Listen(100);

      var args = new SocketAsyncEventArgs();
      args.Completed += AcceptAsyncCompleted;

      _socket.AcceptAsync(args);
    }

    void AcceptAsyncCompleted(object sender, SocketAsyncEventArgs e)
    {
      if (_socket == null) return;

      new Thread(Start).Start(e.AcceptSocket);

      e.AcceptSocket = null;
      _socket.AcceptAsync(e);
    }

    void Start(object newSocket)
    {
      try
      {
        using (var socket = (Socket)newSocket)
        using (var ns = new NetworkStream(socket, FileAccess.Read, false))
          while (_socket != null)
          {
            var logMsg = ReceiverUtils.ParseLog4JXmlLogEvent(ns, "TcpLogger");
            logMsg.LoggerName = string.Format(":{1}.{0}", logMsg.LoggerName, _port);

            if (Notifiable != null)
              Notifiable.Notify(logMsg);
          }
      }
      catch (IOException)
      {
      }
      catch (Exception e)
      {
        Console.WriteLine(e);
      }
    }

    public override void Terminate()
    {
      if (_socket == null) return;

      _socket.Close();
      _socket = null;
    }

    #endregion
  }
}
