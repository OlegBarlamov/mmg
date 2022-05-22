using FrameworkSDK.MonoGame.Config;
using FrameworkSDK.MonoGame.Resources;

namespace FrameworkSDK.MonoGame.Constructing
{
    public interface IGameFactory : IAppFactory
    {
        IGameFactory UseGameParameters(IGameParameters gameParameters);

        IGameFactory PreloadResourcePackage(IResourcePackage resourcePackage);
    }
}