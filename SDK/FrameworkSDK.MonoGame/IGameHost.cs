using FrameworkSDK.MonoGame.Basic;
using FrameworkSDK.MonoGame.Mvc;

namespace FrameworkSDK.MonoGame
{
    internal interface IGameHost : IApplication, IUpdateable, IDrawable, IDisposableExtended
    {
        void OnInitialize();

        void OnLoadContent();

        void OnUnloadContent();
    }
}