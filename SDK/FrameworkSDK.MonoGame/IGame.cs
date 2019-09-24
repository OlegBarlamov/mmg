using System;
using Microsoft.Xna.Framework.Graphics;

namespace FrameworkSDK.MonoGame
{
    public interface IGame : IDisposable
    {
		SpriteBatch SpriteBatch { get; }

        void Run();

        void Stop();
    }
}
