using System;

using Log2Console.Log;


namespace Log2Console.Receiver
{
    public abstract class BaseReceiver : MarshalByRefObject, IReceiver
    {
        protected ILogMessageNotifiable _notifiable = null;


        #region IReceiver Members

        public abstract void Initialize();

        public abstract void Terminate();

        public void Attach(ILogMessageNotifiable notifiable)
        {
            _notifiable = notifiable;
        }

        public void Detach()
        {
            _notifiable = null;
        }

        #endregion
    }
}
