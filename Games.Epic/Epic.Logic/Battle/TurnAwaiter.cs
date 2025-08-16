using System;
using System.Threading.Tasks;
using Epic.Core.Logic;
using JetBrains.Annotations;

namespace Epic.Logic.Battle
{
    internal class TurnAwaiter : IDisposable
    {
        public TurnInfo ExpectedTurn => _expectedTurn;
        
        [CanBeNull] private TurnInfo _expectedTurn;
        [CanBeNull] private TaskCompletionSource<ClientUserAction> _awaitPlayerTurnTaskCompletionSource;

        public Task<ClientUserAction> WaitForClientTurn(int playerIndex, int turnIndex)
        {
            _expectedTurn = new TurnInfo(turnIndex, playerIndex);
            _awaitPlayerTurnTaskCompletionSource = new TaskCompletionSource<ClientUserAction>();
            return _awaitPlayerTurnTaskCompletionSource.Task;
        }

        public void TurnProcessed(string command)
        {
            _awaitPlayerTurnTaskCompletionSource?.SetResult(new ClientUserAction(command));
        } 

        public void Dispose()
        {
            _awaitPlayerTurnTaskCompletionSource?.TrySetCanceled();
        }
        
        internal class ClientUserAction
        {
            public string CommandName { get; }

            public ClientUserAction(string commandName)
            {
                CommandName = commandName;
            }
        }
    }
}