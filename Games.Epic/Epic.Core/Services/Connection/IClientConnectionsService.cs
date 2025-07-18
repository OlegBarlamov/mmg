using Epic.Core.Objects.ClientConnection;

namespace Epic.Core.Services.Connection
{
    public interface IClientConnectionsService<in TConnectionType>
    {
        IClientConnection CreateConnection(TConnectionType connectionInstance);
    }
}