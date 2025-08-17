using System;
using System.Threading.Tasks;
using Epic.Core.ClientMessages;
using Epic.Core.ServerMessages;
using Epic.Core.Services.Battles;

namespace Epic.Core.Services.Connection
{
    public class FakeClientConnection : IBattleClientConnection
    {
        public event Action<IBattleClientConnection, IClientBattleMessage> MessageReceived;
        
        public void Dispose()
        {
            MessageReceived = null;
        }

        public bool IsConnected { get; set; } = true;
        public Guid ConnectionId { get; set; } = Guid.NewGuid();
        public IBattleObject BattleObject { get; set; }

        public FakeClientConnection(IBattleObject battleObject)
        {
            BattleObject = battleObject;
        }
        
        public Task SendMessageAsync(IServerBattleMessage message)
        {
            return Task.CompletedTask;
        }

        public Task CloseAsync()
        {
            return Task.CompletedTask;
        }
    }
}