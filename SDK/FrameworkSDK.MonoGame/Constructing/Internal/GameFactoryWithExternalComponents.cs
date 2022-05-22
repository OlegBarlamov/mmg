using FrameworkSDK.DependencyInjection;
using FrameworkSDK.MonoGame.ExternalComponents;
using JetBrains.Annotations;

namespace FrameworkSDK.MonoGame.Constructing
{
    internal class GameFactoryWithExternalComponents : GameFactoryWrapper, IGameFactoryWithExternalComponents
    {
        public GameFactoryWithExternalComponents([NotNull] IGameFactory gameFactory)
            : base(gameFactory)
        {
        }


        public IGameFactoryWithExternalComponents RegisterExternalGameComponent<TExternalGameComponent>()
            where TExternalGameComponent : class, IExternalGameComponent
        {
            GameFactory.AddServices(new ServicesModuleDelegate(registrator =>
            {
                registrator.RegisterType<IExternalGameComponent, TExternalGameComponent>();
            }));
            return this;
        }
    }
}