using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NetExtensions;
using NetExtensions.Geometry;

namespace FrameworkSDK.MonoGame.Config
{
    [UsedImplicitly]
    public class DefaultGameParameters : IGameParameters
    {
        public string ContentRootDirectory { get; set; } = "Content";
        public SizeInt BackBufferSize { get; set; } = new SizeInt(800, 600);
        public bool IsFullScreenMode { get; set; } = false;
        public GameRunBehavior GameRunBehavior { get; set; } = GameRunBehavior.Synchronous;
        public bool IsMouseVisible { get; set; } = true;
        public GraphicsProfile GraphicsProfile { get; } = GraphicsProfile.Reach;
    }
}
