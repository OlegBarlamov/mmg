using JetBrains.Annotations;

namespace FrameworkSDK.IoC
{
    public interface IServiceContainerFactory
    {
        [NotNull] IFrameworkServiceContainer CreateContainer();
    }
}
