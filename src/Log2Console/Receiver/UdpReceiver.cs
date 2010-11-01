using System;
using System.ComponentModel;
using System.Net;
using System.Net.Sockets;
using System.Threading;

using Log2Console.Log;


namespace Log2Console.Receiver
{
  [Serializable]
  [DisplayName("UDP (IP v4 and v6)")]
  public class UdpReceiver : BaseReceiver
  {
    [NonSerialized]
    private Thread _worker;
    [NonSerialized]
    private UdpClient _udpClient;
    [NonSerialized]
    private IPEndPoint _remoteEndPoint;

    private bool _ipv6;
    private int _port = 7071;
    private string _address = String.Empty;


    [Category("Configuration")]
    [DisplayName("UDP Port Number")]
    [DefaultValue(7071)]
    public int Port
    {
      get { return _port; }
      set { _port = value; }
    }

    [Category("Configuration")]
    [DisplayName("Use IPv6 Addresses")]
    [DefaultValue(false)]
    public bool IpV6
    {
      get { return _ipv6; }
      set { _ipv6 = value; }
    }

    [Category("Configuration")]
    [DisplayName("Multicast Group Address (Optional)")]
    public string Address
    {
      get { return _address; }
      set { _address = value; }
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
      _udpClient = _ipv6 ? new UdpClient(_port, AddressFamily.InterNetworkV6) : new UdpClient(_port);

      if (!String.IsNullOrEmpty(_address))
        _udpClient.JoinMulticastGroup(IPAddress.Parse(_address));

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
          string loggingEvent = System.Text.Encoding.UTF8.GetString(buffer);

          Console.WriteLine(loggingEvent);

          if (Notifiable == null)
            continue;

          LogMessage logMsg = ReceiverUtils.ParseLog4JXmlLogEvent(loggingEvent, "UdpLogger");
          logMsg.LoggerName = string.Format("{0}_{1}", _remoteEndPoint.Address.ToString().Replace(".", "-"), logMsg.LoggerName);
          Notifiable.Notify(logMsg);
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
