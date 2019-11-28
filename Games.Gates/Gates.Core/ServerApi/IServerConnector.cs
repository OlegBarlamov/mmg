using System.Threading;
using System.Threading.Tasks;

namespace Gates.Core.ServerApi
{
    public interface IServerConnector
    {
        Task<IServer> ConnectAsync(string url, CancellationToken cancellationToken);
    }
}
