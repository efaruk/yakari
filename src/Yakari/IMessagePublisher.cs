using System.Threading;
using System.Threading.Tasks;

namespace Yakari
{
    public interface IMessagePublisher
    {
        Task PublishAsync(string message, CancellationToken cancellationToken = default);
    }
}