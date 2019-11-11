using Gates.Core.ServerApi;

namespace Gates.ClientCore.ServerConnection
{
    internal interface IServerConnector
    {
        IServer Connect(string url);
    }
}
