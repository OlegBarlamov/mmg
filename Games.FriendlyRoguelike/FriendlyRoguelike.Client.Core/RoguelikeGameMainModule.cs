using FrameworkSDK.DependencyInjection;
using FriendlyRoguelike.Core.Components;
using FriendlyRoguelike.Core.Models;
using FriendlyRoguelike.Core.Services;
using FriendlyRoguelike.Core.Services.Internal;

namespace FriendlyRoguelike.Core
{
    public class RoguelikeGameMainModule : IServicesModule
    {
        public void RegisterServices(IServiceRegistrator serviceRegistrator)
        {
            var gameRootModel = new GameRootModel();
            serviceRegistrator.RegisterInstance(gameRootModel);
            serviceRegistrator.RegisterInstance<IGameRootModelRead>(gameRootModel);
            
            serviceRegistrator.RegisterType<IGameStagesService, GameStagesServicesImpl>();
            serviceRegistrator.RegisterType<GameMainComponent, GameMainComponent>();
            serviceRegistrator.RegisterType<StartingGameComponent, StartingGameComponent>();
        }
    }
}