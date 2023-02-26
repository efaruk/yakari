using System.Threading;
using System.Threading.Tasks;

namespace Yakari
{
    public interface IMessageSubscriber
    {
        Task StartSubscriptionAsync(CancellationToken cancellationToken = default);

        Task StopSubscriptionAsync(CancellationToken cancellationToken = default);

        Task MessageReceivedAsync(string message, CancellationToken cancellationToken = default);

        event MessageReceived OnMessageReceived;
    }

    public delegate Task MessageReceived(string message, CancellationToken cancellationToken = default);
}