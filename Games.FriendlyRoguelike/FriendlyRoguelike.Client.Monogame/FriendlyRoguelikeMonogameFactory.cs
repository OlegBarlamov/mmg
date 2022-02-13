using Console.FrameworkAdapter;
using FrameworkSDK;
using FrameworkSDK.Constructing;
using FrameworkSDK.MonoGame.Config;
using FrameworkSDK.MonoGame.Constructing;
using FrameworkSDK.MonoGame.ExternalComponents;
using FriendlyRoguelike.Core;

namespace FriendlyRoguelike.Client.Monogame
{
    public static class RoguelikeGameFactory
    {
        public static IAppFactory Create()
        {
            var coreConfigurator = Core.RoguelikeGameFactory.Create(new RoguelikeGameFactoryConfig());
            return coreConfigurator
                .AddServices<MonoGameSpecificModule>()
                .UseGame<RoguelikeMainGameClass>()
                .UseGameParameters(GetParameters())
                .UseGameComponents()
                .UseInGameConsole()
                .UseConsoleMessagesProvider(coreConfigurator.ConsoleMessagesProvider);
        }

        private static IGameParameters GetParameters()
        {
            return new DefaultGameParameters
            {
                ContentRootDirectory = "Resources"
            };
        }
    }
}