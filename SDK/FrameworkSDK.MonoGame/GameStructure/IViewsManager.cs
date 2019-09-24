using FrameworkSDK.MonoGame.GameStructure.Views;
using JetBrains.Annotations;

namespace FrameworkSDK.MonoGame.GameStructure
{
    public interface IViewsManager
    {
        void AddView([NotNull] IView view);

        void RemoveView([NotNull] IView view);
    }
}