using FrameworkSDK.MonoGame.Mvc;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using NetExtensions;

namespace FrameworkSDK.MonoGame.Config
{
    [UsedImplicitly]
    internal class DefaultGameParameters : IGameParameters
    {
        public string ContentRootDirectory { get; } = "Content";
        public Int32Size BackBufferSize { get; } = new Int32Size(1280, 920);
        public bool IsFullScreenMode { get; } = false;
        public GameRunBehavior GameRunBehavior { get; } = GameRunBehavior.Synchronous;
    }
}
