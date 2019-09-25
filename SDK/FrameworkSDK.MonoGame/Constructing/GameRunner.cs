using System;
using FrameworkSDK.Constructing;
using FrameworkSDK.IoC;
using FrameworkSDK.Localization;
using JetBrains.Annotations;

namespace FrameworkSDK.MonoGame.Constructing
{
    internal class GameRunner<TGame> : IAppRunner where TGame : IGameHost
    {
        private IAppRunner AppRunner { get; }

        public GameRunner([NotNull] IAppRunner appRunner)
        {
            AppRunner = appRunner ?? throw new ArgumentNullException(nameof(appRunner));
        }

        public void Run()
        {
            try
            {
                var locator = AppContext.ServiceLocator;
                var gameHost = locator.Resolve<TGame>();
                var gameHeart = locator.Resolve<IGameHeart>();

                gameHost.Initialize(gameHeart);
            }
            catch (Exception e)
            {
                throw new GameConstructingException(Strings.Exceptions.Constructing.RunAppFailed, e,
                    typeof(TGame).Name);
            }

            AppRunner.Run();
        }

        public void Dispose()
        {
            AppRunner.Dispose();
        }
    }
}