using FrameworkSDK.Game;
using FrameworkSDK.Game.Scenes;

namespace FrameworkSDK
{
    public interface IGameHost
    {
        Scene CurrentScene { get; }

        void Run(IGame game);
    }
}
