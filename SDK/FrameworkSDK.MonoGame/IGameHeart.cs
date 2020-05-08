using System;

namespace FrameworkSDK.MonoGame
{
    internal interface IGameHeart
    {
        event Action ResourceLoading;
        event Action ResourceUnloading;
        
        void Run();
    }
}
