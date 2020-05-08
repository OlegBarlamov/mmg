using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace FrameworkSDK.MonoGame.Core
{
    public interface IGameHeartServices : IDisposable
    {
        event Action ResourceLoading;
        event Action ResourceUnloading;
        event Action Loaded;
        
        bool IsServicesLoaded { get; }
        
        GraphicsDeviceManager GraphicsDeviceManager { get; }
        ContentManager RootContentManager { get; }
        SpriteBatch SpriteBatch { get; }
        GameServiceContainer MonoGameServiceContainer { get; }
    }
}