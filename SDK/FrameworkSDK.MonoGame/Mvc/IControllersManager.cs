using JetBrains.Annotations;

namespace FrameworkSDK.MonoGame.Mvc
{
    public interface IControllersManager
    {
        void AddController([NotNull] IController controller);

        void RemoveController([NotNull] IController controller);
    }
}
