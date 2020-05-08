using System;
using FrameworkSDK.MonoGame.Graphics.RenderingTools;
using Microsoft.Xna.Framework;
using IDrawable = FrameworkSDK.MonoGame.Basic.IDrawable;
using IUpdateable = FrameworkSDK.MonoGame.Basic.IUpdateable;

namespace FrameworkSDK.MonoGame.ExternalComponents
{
    public interface IExternalGameComponent : IUpdateable, IDrawable, IDisposable
    {
        void LoadContent();
        
        void Initialize();

        void UnloadContent();
    }
}