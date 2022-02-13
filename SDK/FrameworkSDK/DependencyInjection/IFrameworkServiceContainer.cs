using System;
using JetBrains.Annotations;

namespace FrameworkSDK.DependencyInjection
{
    public interface IFrameworkServiceContainer : IServiceRegistrator, IDisposable
    {
        [NotNull] IServiceLocator BuildContainer();

        /// <summary>
        /// Дочерний контейнер имеет досутп к регистрациям родительского, включая кэширование singletone сервисов.
        /// Однако, при уничтожении дочернего контейнера, не уничтожаются сервисы, зарегстрированные в родительском.
        /// Как и при уничтожении родительского, дочерние регистрации продолжают жить.
        /// </summary>
	    [NotNull] IFrameworkServiceContainer CreateScoped(string name = null);
    }
}
