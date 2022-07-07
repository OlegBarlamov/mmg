using System;
using System.Threading;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;

namespace FrameworkSDK.MonoGame.Core
{
    public class SimpleDelayedTask : IDelayedTask
    {
        public bool Cancelled => CancellationToken.IsCancellationRequested;

        private Action<GameTime, CancellationToken> ExecuteAction { get; }
        public CancellationToken CancellationToken { get; }

        public SimpleDelayedTask([NotNull] Action<GameTime> executeAction, CancellationToken cancellationToken)
            :this((time, token) => executeAction(time), cancellationToken)
        {
        }
        
        public SimpleDelayedTask([NotNull] Action<GameTime, CancellationToken> executeAction, CancellationToken cancellationToken)
        {
            ExecuteAction = executeAction ?? throw new ArgumentNullException(nameof(executeAction));
            CancellationToken = cancellationToken;
        }
        
        public void Execute(GameTime gameTime)
        {
            ExecuteAction(gameTime, CancellationToken);
        }

        private static readonly GameTime EmptyGameTime = new GameTime();
        public void Execute()
        {
            ExecuteAction(EmptyGameTime, CancellationToken);
        }
    }
}