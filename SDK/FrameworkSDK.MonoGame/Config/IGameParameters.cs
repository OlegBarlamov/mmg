using Microsoft.Xna.Framework;
using NetExtensions;
using NetExtensions.Geometry;

namespace FrameworkSDK.MonoGame.Config
{
    public interface IGameParameters
    {
        string ContentRootDirectory { get; }
        SizeInt BackBufferSize { get; }
        bool IsFullScreenMode { get; }
        GameRunBehavior GameRunBehavior { get; }
        bool IsMouseVisible { get; }
    }
}
