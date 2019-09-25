using System;
using FrameworkSDK.MonoGame.Mvc;
using JetBrains.Annotations;

namespace FrameworkSDK.MonoGame
{
    public interface IGameHost : IApplication, IUpdateable, IDrawable, IDisposable
    {
        void Initialize([NotNull] IGameHeart gameHeart);
    }
}
