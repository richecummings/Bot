using System.Threading;
using System.Threading.Tasks;

namespace Bot.Configuration
{
    public interface IConfigurationManager
    {
        Task<Configuration> GetAsync(CancellationToken cancellationToken);
    }
}
