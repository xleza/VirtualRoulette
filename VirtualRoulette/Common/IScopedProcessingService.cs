using System.Threading;
using System.Threading.Tasks;

namespace VirtualRoulette.Common
{
    public interface IScopedProcessingService
    {
        Task DoWorkAsync(CancellationToken stoppingToken);
    }
}
