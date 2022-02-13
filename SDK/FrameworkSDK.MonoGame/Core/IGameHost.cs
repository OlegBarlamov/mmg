using FrameworkSDK.Common;
using FrameworkSDK.MonoGame.Basic;

namespace FrameworkSDK.MonoGame
{
    internal interface IGameHost : IUpdatable, IDrawable, IDisposableExtended
    {
        void OnInitialize();

        void OnLoadContent();

        void OnUnloadContent();
    }
}