using System;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FrameworkSDK.MonoGame
{
    public interface IGameHeart : IDisposable
    {
        //TODO Вынести это куда то?
        SpriteBatch SpriteBatch { get; }
        GraphicsDeviceManager GraphicsDeviceManager { get; }

        void Run([NotNull] IGameHost gameHost);

        void Stop();
    }
}
