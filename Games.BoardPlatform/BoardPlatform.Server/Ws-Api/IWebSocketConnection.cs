using System;
using System.Threading;
using System.Threading.Tasks;
using BoardPlatform.Server.Data;

namespace BoardPlatform.Server
{
    public interface IWebSocketConnection : IDisposable
    {
        public ClientAppInfo ClientAppInfo { get; }
        
        event Action ConnectionLost;
        event Action<IWsClientToServerMessage> MessageReceived;
        
        bool IsRunning { get; }
        
        WsConnectionStatus Status { get; }

        Task Establish(CancellationToken cancellationToken);

        void Close();

        void SendMessage(IWsServerToClientMessage message);

        Task<IWsClientToServerMessage> WaitForCommandAsync(WsClientToServerCommand command,
            CancellationToken cancellationToken);
    }
}