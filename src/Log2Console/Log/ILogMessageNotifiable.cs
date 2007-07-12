
namespace Log2Console.Log
{
    /// <summary>
    /// This interface must be implemented to be notified when new log message arrives.
    /// </summary>
    public interface ILogMessageNotifiable
	{
		/// <summary>
		/// Call this method when new log messages are arrived.
		/// </summary>
		/// <param name="logMsgs">The messages to log.</param>
		void Notify(LogMessage[] logMsgs);

        /// <summary>
        /// Call this method when a new log message is arrived.
        /// </summary>
        /// <param name="logMsg">The message to log.</param>
        void Notify(LogMessage logMsg);
    }
}
