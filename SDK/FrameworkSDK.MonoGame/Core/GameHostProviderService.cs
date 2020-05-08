using System;
using JetBrains.Annotations;

namespace FrameworkSDK.MonoGame.Core
{
    [UsedImplicitly]
    internal class GameHostProviderService<TGameHost> : IGameHostProvider where TGameHost : class, IGameHost
    {
        public IGameHost GameHost { get; }

        public GameHostProviderService([NotNull] TGameHost gameHost)
        {
            GameHost = gameHost ?? throw new ArgumentNullException(nameof(gameHost));
        }
    }
}