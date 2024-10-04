using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Epic.Core.Objects.Battle;
using Epic.Core.Objects.BattleClientConnection;
using JetBrains.Annotations;

namespace Epic.Core.Objects.BattleGameManager
{
    public class BattleGameManager : IBattleGameManager
    {
        public bool IsDisposed { get; private set; }
        public IBattleObject BattleObject { get; set; }

        private readonly object _clientConnectionsLock = new object();
        private readonly List<IBattleClientConnection> _clientConnections = new List<IBattleClientConnection>();
        
        public BattleGameManager([NotNull] IBattleObject battleObject)
        {
            BattleObject = battleObject ?? throw new ArgumentNullException(nameof(battleObject));
        }
        
        public void Dispose()
        {
            IsDisposed = true;
        }

        public int GetClientsCount()
        {
            lock (_clientConnectionsLock)
            {
                return _clientConnections.Count;
            }
        }

        public Task PlayBattle()
        {
            throw new System.NotImplementedException();
        }

        public Task SuspendBattle()
        {
            throw new System.NotImplementedException();
        }

        public Task AddClient(IBattleClientConnection connection)
        {
            lock (_clientConnectionsLock)
            {
                _clientConnections.Add(connection);
            }
            return Task.CompletedTask;
        }

        public Task RemoveClient(IBattleClientConnection connection)
        {
            lock (_clientConnectionsLock)
            {
                _clientConnections.Remove(connection);
            }
            return Task.CompletedTask;
        }
    }
}