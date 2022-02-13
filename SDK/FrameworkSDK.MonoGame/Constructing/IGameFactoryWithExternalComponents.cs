using FrameworkSDK.MonoGame.ExternalComponents;

namespace FrameworkSDK.MonoGame.Constructing
{
    public interface IGameFactoryWithExternalComponents : IAppFactory
    {
        IGameFactoryWithExternalComponents RegisterExternalGameComponent<TExternalGameComponent>() where TExternalGameComponent : class, IExternalGameComponent;
    }
}