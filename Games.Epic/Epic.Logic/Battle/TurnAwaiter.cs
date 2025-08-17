using System;
using System.Threading.Tasks;
using Epic.Core.Logic;
using JetBrains.Annotations;

namespace Epic.Logic.Battle
{
    internal class TurnAwaiter : IDisposable
    {
        public TurnInfo ExpectedTurn => _expectedTurn;
        
        private bool _aiTurnExpected;
        [CanBeNull] private TurnInfo _expectedTurn;
        [CanBeNull] private TaskCompletionSource<ClientUserAction> _awaitPlayerTurnTaskCompletionSource;

        public Task<ClientUserAction> WaitForClientTurn(int playerIndex, int turnIndex)
        {
            _aiTurnExpected = false;
            _expectedTurn = new TurnInfo(turnIndex, playerIndex);
            _awaitPlayerTurnTaskCompletionSource = new TaskCompletionSource<ClientUserAction>();
            return _awaitPlayerTurnTaskCompletionSource.Task;
        }

        public void WaitForAiTurn(int playerIndex, int turnIndex)
        {
            _expectedTurn = new TurnInfo(turnIndex, playerIndex);
            _aiTurnExpected = true;
        }

        public void TurnProcessed(string command)
        {
            if (!_aiTurnExpected)
            {
                var completionSource = _awaitPlayerTurnTaskCompletionSource;  
                _awaitPlayerTurnTaskCompletionSource = null;
                completionSource?.TrySetResult(new ClientUserAction(command));
            }
            _aiTurnExpected = false;
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