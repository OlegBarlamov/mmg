using JetBrains.Annotations;

namespace FrameworkSDK.MonoGame.Mvc
{
    public interface IViewsManager
    {
        void AddView([NotNull] IView view);

        void RemoveView([NotNull] IView view);
    }
}