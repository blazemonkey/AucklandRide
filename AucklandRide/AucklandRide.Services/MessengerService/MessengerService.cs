using GalaSoft.MvvmLight.Messaging;
using System;

namespace Services.MessengerService
{
    public class MessengerService : IMessengerService
    {
        public void Register<T>(object recipient, object token, Action<T> action)
        {
            Messenger.Default.Register(recipient, token, action);
        }

        public void Unregister<T>(object recipient, object token, Action<T> action)
        {
            Messenger.Default.Unregister(recipient, token, action);
        }

        public void Send<T>(T message, object token)
        {
            Messenger.Default.Send(message, token);
        }
    }
}
