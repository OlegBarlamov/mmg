using FrameworkSDK.DependencyInjection;
using JetBrains.Annotations;

namespace FrameworkSDK.MonoGame.Services.Implementations
{
    [UsedImplicitly]
    internal class DefaultAppTerminator : IAppTerminator
    {
        /// <summary>
        /// To be able to request the service in GameHost constructor
        /// </summary>
        private IGameHeart GameHeart => AppContext.ServiceLocator.Resolve<IGameHeart>();

        public void Terminate()
        {
            GameHeart.Exit();
        }
    }
}