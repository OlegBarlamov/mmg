namespace FrameworkSDK.IoC.Default
{
    internal class DefaultServiceContainerFactory : IServiceContainerFactory
    {
        public IFrameworkServiceContainer CreateContainer()
        {
            return new DefaultServiceContainer();
        }
    }
}
