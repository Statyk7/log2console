## Log4net Configuration Block Sample
Here is a sample of a log4net configuration block to be set in your project in order to send log events to the Log2Console.
Note that is you change the sink value, you have to update the Log2Console config file in order to match the new settings.

{{
<?xml version="1.0" encoding="utf-8" ?>
<configuration>
  <!-- Register a section handler for the log4net section -->
  <configSections>
    <section name="log4net" type="System.Configuration.IgnoreSectionHandler" />
  </configSections>

  <!-- This section contains the log4net configuration settings -->
  <log4net>
    <!-- Define a Remoting Appender, to be used with Log2Console -->
    <appender name="RemotingAppender" type="log4net.Appender.RemotingAppender" >
      <!-- The remoting URL to the remoting server object -->
      <sink value="tcp://localhost:7070/LoggingSink" />
      <!-- Send all events, do not discard events when the buffer is full -->
      <lossy value="false" />
      <!-- The number of events to buffer before sending -->
      <bufferSize value="1" />
      <!-- Specify an evaluator to send the events immediatly under
			     certain conditions, e.g. when an error event ocurrs -->
			<!--<evaluator type="log4net.Core.LevelEvaluator">
				<threshold value="ERROR"/>
			</evaluator>-->
    </appender>

    <!-- Setup the root category, add the appenders and set the default level -->
    <root>
      <level value="DEBUG" />
      <appender-ref ref="RemotingAppender" />
    </root>
  </log4net>
</configuration>
}}

## Usage in Code
Then in your code (in Program.cs for instance), add this lines:

{{
using log4net;

// Configure log4net using the .config file
[assembly: log4net.Config.XmlConfigurator(Watch = true)](assembly_-log4net.Config.XmlConfigurator(Watch-=-true))
}}

## More
More info here: [http://logging.apache.org/log4net/](http://logging.apache.org/log4net/)