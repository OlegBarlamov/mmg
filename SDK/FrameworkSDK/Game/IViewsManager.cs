using FrameworkSDK.Game.Views;
using JetBrains.Annotations;

namespace FrameworkSDK.Game
{
    public interface IViewsManager
    {
        void AddView([NotNull] IView view);

        void RemoveView([NotNull] IView view);
    }
}