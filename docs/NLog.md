## NLog configuration

As describe [here](http://log2console.codeplex.com/WorkItem/View.aspx?WorkItemId=818), it's possible to use Log2Console with NLog.

NLog Target: <target name="chainsaw" xsi:type="Chainsaw" address="udp://localhost:7071" />
Log2Console Receiver: UDP Receiver, localhost, port 7071