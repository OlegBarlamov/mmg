using FrameworkSDK.MonoGame.GameStructure.Controllers;
using FrameworkSDK.MonoGame.GameStructure.Views;
using JetBrains.Annotations;

namespace FrameworkSDK.MonoGame.GameStructure.Mapping
{
    public interface IMvcScheme
    {
        [CanBeNull] object Model { get; }
        [CanBeNull] IController Controller { get; }
        [CanBeNull] IView View { get; }
    }
}