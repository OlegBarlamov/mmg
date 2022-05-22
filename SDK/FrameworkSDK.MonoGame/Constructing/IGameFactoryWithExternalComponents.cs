using FrameworkSDK.MonoGame.ExternalComponents;

namespace FrameworkSDK.MonoGame.Constructing
{
    public interface IGameFactoryWithExternalComponents : IGameFactory
    {
        IGameFactoryWithExternalComponents RegisterExternalGameComponent<TExternalGameComponent>() where TExternalGameComponent : class, IExternalGameComponent;
    }
}