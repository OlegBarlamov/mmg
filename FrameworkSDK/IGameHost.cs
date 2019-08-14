using System;
using FrameworkSDK.Game;
using FrameworkSDK.Game.Scenes;
using JetBrains.Annotations;

namespace FrameworkSDK
{
    public interface IGameHost : IUpdatable, IDisposable
    {
        [CanBeNull] Scene CurrentScene { get; }

        void Run([NotNull] IGame game);
    }
}
