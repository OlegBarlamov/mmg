using System.Threading;
using System.Threading.Tasks;

namespace Gates.Core.ServerApi
{
    public interface IServer
    {
        Task<IServerApi> AuthorizeAsync(string name, string password, CancellationToken cancellationToken);
    }
}
