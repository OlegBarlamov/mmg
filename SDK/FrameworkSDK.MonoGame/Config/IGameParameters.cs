using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
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
        GraphicsProfile GraphicsProfile { get; }
        bool IsFixedTimeStamp { get; }
        bool SynchronizeWithVerticalRetrace { get; }
        TimeSpan TargetElapsedTime { get; } 
    }
}
