using FrameworkSDK.MonoGame.Basic;
using JetBrains.Annotations;

// ReSharper disable once CheckNamespace
namespace FrameworkSDK.MonoGame.Mvc
{
    public interface ISceneExtension : IUpdatable, INamed
    {
        void OnClosed();

        void OnOpened();

        void OnOpening();

        void OnViewAttached([NotNull] IView view);

        void OnViewDetached([NotNull] IView view);

        void OnControllerAttached([NotNull] IController controller);

        void OnControllerDetached([NotNull] IController controller);
    }
}