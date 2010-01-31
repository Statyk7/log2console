using System;

using Log2Console.Log;


namespace Log2Console.Receiver
{
    [Serializable]
    public abstract class BaseReceiver : MarshalByRefObject, IReceiver
    {
        [NonSerialized]
        protected ILogMessageNotifiable Notifiable;


        #region IReceiver Members

        public abstract string SampleClientConfig { get; }

        public abstract void Initialize();
        public abstract void Terminate();

        public virtual void Attach(ILogMessageNotifiable notifiable)
        {
            Notifiable = notifiable;
        }

        public virtual void Detach()
        {
            Notifiable = null;
        }

        #endregion
    }
}
