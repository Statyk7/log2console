using Log2Console.Log;


namespace Log2Console.Receiver
{
    public interface IReceiver
    {
        string SampleClientConfig { get; }

		void Initialize();
		void Terminate();

        void Attach(ILogMessageNotifiable notifiable);
        void Detach();
    }
}
