using FrameworkSDK.Constructing;
using FrameworkSDK.DependencyInjection;
using FrameworkSDK.MonoGame.Config;
using JetBrains.Annotations;

namespace FrameworkSDK.MonoGame.Constructing
{
    internal class GameFactory : AppFactoryWrapper, IGameFactory
    {
        public GameFactory([NotNull] IAppFactory appFactory)
            : base(appFactory)
        {
        }

        public IGameFactory UseGameParameters(IGameParameters gameParameters)
        {
            AppFactory.AddServices(new ServicesModuleDelegate(registrator =>
            {
                registrator.RegisterInstance(gameParameters);
            }));
            return this;
        }
    }
}