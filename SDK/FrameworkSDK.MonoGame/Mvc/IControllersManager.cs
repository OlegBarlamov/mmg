using JetBrains.Annotations;

namespace FrameworkSDK.MonoGame.Mvc
{
    public interface IControllersManager
    {
        void AddController([NotNull] IController controller);
        
        IController AddController([NotNull] object model);

        void RemoveController([NotNull] IController controller);
        IController RemoveController([NotNull] object model);
    }
}
