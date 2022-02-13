using FrameworkSDK.Constructing;
using FrameworkSDK.DependencyInjection;
using FrameworkSDK.MonoGame.ExternalComponents;
using JetBrains.Annotations;

namespace FrameworkSDK.MonoGame.Constructing
{
    internal class GameFactoryWithExternalComponents : AppFactoryWrapper, IGameFactoryWithExternalComponents
    {
        [NotNull] public IGameFactory GameFactory { get; }

        public GameFactoryWithExternalComponents([NotNull] IGameFactory gameFactory)
            : base(gameFactory)
        {
            GameFactory = gameFactory;
        }


        public IGameFactoryWithExternalComponents RegisterExternalGameComponent<TExternalGameComponent>()
            where TExternalGameComponent : class, IExternalGameComponent
        {
            AppFactory.AddServices(new ServicesModuleDelegate(registrator =>
            {
                registrator.RegisterType<IExternalGameComponent, TExternalGameComponent>();
            }));
            return this;
        }
    }
}