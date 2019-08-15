using JetBrains.Annotations;

namespace FrameworkSDK.IoC
{
    public interface IFrameworkServiceContainer : IServiceRegistrator
    {
        [NotNull] IServiceLocator BuildContainer();

	    IFrameworkServiceContainer Clone();
    }
}
