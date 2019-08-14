using FrameworkSDK.Game.Controllers;

namespace FrameworkSDK.Game
{
    public interface IControllersManager
    {
        void AddController(IController controller);

        void RemoveController(IController controller);
    }
}
