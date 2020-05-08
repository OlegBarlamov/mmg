using System;
using FrameworkSDK.Constructing;
using FrameworkSDK.IoC;
using FrameworkSDK.MonoGame.Localization;
using JetBrains.Annotations;

namespace FrameworkSDK.MonoGame.Constructing
{
    internal class GameRunner<TGame> : IAppRunner where TGame : GameApp
    {
        private IAppRunner AppRunner { get; }

        public GameRunner([NotNull] IAppRunner appRunner)
        {
            AppRunner = appRunner ?? throw new ArgumentNullException(nameof(appRunner));
        }

        public void Run()
        {
            IGameHeart gameHeart;
            
            try
            {
                var locator = AppContext.ServiceLocator;
                gameHeart = locator.Resolve<IGameHeart>();

                AppRunner.Run();
            }
            catch (Exception e)
            {
                throw new GameConstructingException(Strings.Exceptions.Constructing.RunGameFailed, e,
                    typeof(TGame).Name);
            }

            try
            {
                gameHeart.Run();
            }
            catch (Exception e)
            {
                throw new FrameworkMonoGameException(Strings.Exceptions.FatalException, e);
            }
        }

        public void Dispose()
        {
            AppRunner.Dispose();
        }
    }
}