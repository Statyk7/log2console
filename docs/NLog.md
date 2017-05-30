## NLog configuration

It is possible to use Log2Console with NLog.

NLog Target: <target name="chainsaw" xsi:type="Chainsaw" address="udp://localhost:7071" />
Log2Console Receiver: UDP Receiver, localhost, port 7071
