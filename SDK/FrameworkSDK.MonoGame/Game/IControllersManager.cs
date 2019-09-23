using FrameworkSDK.Game.Controllers;
using JetBrains.Annotations;

namespace FrameworkSDK.Game
{
    public interface IControllersManager
    {
        void AddController([NotNull] IController controller);

        void RemoveController([NotNull] IController controller);
    }
}
