namespace Yakari.Core.Interfaces
{
    public interface IMessagePublisher
    {
        void Publish(string message);
    }
}