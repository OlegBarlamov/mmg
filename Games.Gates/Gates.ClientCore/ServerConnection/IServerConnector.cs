using System;
using Gates.Core.ServerApi;

namespace Gates.ClientCore.ServerConnection
{
    internal interface IServerConnector
    {
        IServer Connect(string url);
    }

    internal class FakeServerConnector : IServerConnector
    {
        public IServer Connect(string url)
        {
            throw new NotImplementedException();
        }
    }
}
