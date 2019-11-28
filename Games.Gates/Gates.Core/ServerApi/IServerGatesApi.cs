using System.Threading;
using System.Threading.Tasks;
using Gates.Core.Models;

namespace Gates.Core.ServerApi
{
    public interface IServerGatesApi
    {
        Task PostUserReadyAsync(CancellationToken cancellationToken);

        Task<GameStartInfo> GetStartInfoAsync(CancellationToken cancellationToken);

        Task<GameDataIteration> GemeStateAsync(CancellationToken cancellationToken);
    }
}
