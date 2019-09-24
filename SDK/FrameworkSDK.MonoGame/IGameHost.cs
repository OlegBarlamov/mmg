using System;
using FrameworkSDK.MonoGame.GameStructure;
using FrameworkSDK.MonoGame.GameStructure.Scenes;
using JetBrains.Annotations;

namespace FrameworkSDK
{
    public interface IGameHost : IUpdatable, IDisposable
    {
        [CanBeNull] Scene CurrentScene { get; }

        void Run([NotNull] IGame game);
    }
}
