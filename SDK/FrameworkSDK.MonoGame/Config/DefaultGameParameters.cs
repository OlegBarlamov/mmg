using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using NetExtensions;

namespace FrameworkSDK.MonoGame.Config
{
    [UsedImplicitly]
    public class DefaultGameParameters : IGameParameters
    {
        public string ContentRootDirectory { get; set; } = "Content";
        public Int32Size BackBufferSize { get; set; } = new Int32Size(800, 600);
        public bool IsFullScreenMode { get; set; } = false;
        public GameRunBehavior GameRunBehavior { get; set; } = GameRunBehavior.Synchronous;
        public bool IsMouseVisible { get; set; } = true;
    }
}
