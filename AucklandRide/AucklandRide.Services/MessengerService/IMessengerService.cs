using System;

namespace Services.MessengerService
{
    public interface IMessengerService
    {
        void Register<T>(object recipient, object token, Action<T> action);
        void Unregister<T>(object recipient, object token, Action<T> action);
        void Send<T>(T message, object token);
    }
}
