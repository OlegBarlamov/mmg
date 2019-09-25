using FrameworkSDK.MonoGame.Mvc;
using JetBrains.Annotations;

namespace FrameworkSDK.MonoGame.Mvc
{
    public interface IMvcScheme
    {
        [CanBeNull] object Model { get; }
        [CanBeNull] IController Controller { get; }
        [CanBeNull] IView View { get; }
    }
}