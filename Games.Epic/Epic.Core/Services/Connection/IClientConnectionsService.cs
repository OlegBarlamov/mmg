using Epic.Core.Objects.ClientConnection;

namespace Epic.Core
{
    public interface IClientConnectionsService<in TConnectionType>
    {
        IClientConnection CreateConnection(TConnectionType connectionInstance);
    }
}