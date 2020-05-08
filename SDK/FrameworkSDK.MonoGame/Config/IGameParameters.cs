using Microsoft.Xna.Framework;
using NetExtensions;

namespace FrameworkSDK.MonoGame.Config
{
    public interface IGameParameters
    {
        string ContentRootDirectory { get; }
        Int32Size BackBufferSize { get; }
        bool IsFullScreenMode { get; }
        GameRunBehavior GameRunBehavior { get; }
        bool IsMouseVisible { get; }
    }
}
