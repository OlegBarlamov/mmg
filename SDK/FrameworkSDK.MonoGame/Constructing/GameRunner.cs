using System;
using FrameworkSDK.Constructing;
using FrameworkSDK.IoC;
using FrameworkSDK.Localization;
using JetBrains.Annotations;

namespace FrameworkSDK.MonoGame.Constructing
{
    internal class GameRunner<TGame> : IAppRunner
    {
        private IAppRunner AppRunner { get; }

        public GameRunner([NotNull] IAppRunner appRunner)
        {
            AppRunner = appRunner ?? throw new ArgumentNullException(nameof(appRunner));
        }

        public void Run()
        {
            IGameHost gameHost;
            IGame game;

            try
            {
                AppRunner.Run();

                var locator = AppContext.ServiceLocator;
                gameHost = locator.Resolve<IGameHost>();
                game = locator.Resolve<IGame>();
            }
            catch (Exception e)
            {
                throw new GameConstructingException(Strings.Exceptions.Constructing.RunAppFailed, e,
                    typeof(TGame).Name);
            }

            gameHost.Run(game);
        }

        public void Dispose()
        {
            AppRunner.Dispose();
        }
    }
}