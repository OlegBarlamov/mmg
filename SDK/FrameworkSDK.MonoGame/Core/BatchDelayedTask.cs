using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace FrameworkSDK.MonoGame.Core
{
    public class BatchDelayedTask : IDelayedTask
    {
        public bool Cancelled => _cancelledPredicate();
        public bool IsCompleted => _currentIndex >= _actions.Count;

        private readonly IReadOnlyList<Action> _actions;
        private readonly Func<bool> _cancelledPredicate;
        private int _currentIndex;

        public BatchDelayedTask([NotNull] IReadOnlyList<Action> actions, [NotNull] Func<bool> cancelledPredicate)
        {
            _actions = actions ?? throw new ArgumentNullException(nameof(actions));
            _cancelledPredicate = cancelledPredicate ?? throw new ArgumentNullException(nameof(cancelledPredicate));
        }

        public void Execute()
        {
            if (IsCompleted)
                return;

            _actions[_currentIndex]();
            _currentIndex++;
        }
    }
}
