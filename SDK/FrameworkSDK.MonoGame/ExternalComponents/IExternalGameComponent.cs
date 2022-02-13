using System;
using FrameworkSDK.MonoGame.Basic;
using FrameworkSDK.MonoGame.Graphics.RenderingTools;
using Microsoft.Xna.Framework;
using IDrawable = FrameworkSDK.MonoGame.Basic.IDrawable;

namespace FrameworkSDK.MonoGame.ExternalComponents
{
    public interface IExternalGameComponent : IUpdatable, IDrawable, IDisposable
    {
        void LoadContent();
        
        void Initialize();

        void UnloadContent();
    }
}