using FrameworkSDK.MonoGame.Config;

namespace FrameworkSDK.MonoGame.Constructing
{
    public interface IGameFactory : IAppFactory
    {
        IGameFactory UseGameParameters(IGameParameters gameParameters);
    }
}