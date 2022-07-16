using JetBrains.Annotations;

namespace FrameworkSDK.MonoGame.Mvc
{
    public interface IViewsManager
    {
        void AddView([NotNull] IView view);
        
        IView AddView([NotNull] object model);

        void RemoveView([NotNull] IView view);
        
        IView RemoveView([NotNull] object model);
    }
}