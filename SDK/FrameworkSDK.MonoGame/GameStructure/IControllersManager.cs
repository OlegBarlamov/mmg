using FrameworkSDK.MonoGame.GameStructure.Controllers;
using JetBrains.Annotations;

namespace FrameworkSDK.MonoGame.GameStructure
{
    public interface IControllersManager
    {
        void AddController([NotNull] IController controller);

        void RemoveController([NotNull] IController controller);
    }
}
