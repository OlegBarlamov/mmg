using Console.FrameworkAdapter;
using FrameworkSDK.Constructing;
using FrameworkSDK.MonoGame.Config;
using FrameworkSDK.MonoGame.Constructing;
using FrameworkSDK.MonoGame.ExternalComponents;
using FriendlyRoguelike.Core;

namespace FriendlyRoguelike.Client.Monogame
{
    public static class RoguelikeGameFactory
    {
        public static IAppRunner Create()
        {
            var coreConfigurator = Core.RoguelikeGameFactory.Create(new RoguelikeGameFactoryConfig());
            return coreConfigurator
                .UseGame<RoguelikeMainGameClass>()
                .SetupGameParameters(GetParameters)
                .UseComponents()
                .AddConsole(coreConfigurator.ConsoleMessagesProvider)
                .RegisterServices<MonoGameSpecificModule>()
                .Configure();
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