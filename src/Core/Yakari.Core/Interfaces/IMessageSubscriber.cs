namespace Yakari.Core.Interfaces
{
    public interface IMessageSubscriber
    {
        void StartSubscription();

        void StopSubscription();

        void MessageReceived(string message);

        event MessageReceived OnMessageReceived;
    }

    public delegate void MessageReceived(string message);
}