namespace FrameworkSDK.IoC.Default
{
    internal class DefaultServiceContainerFactory : IServiceContainerFactory
    {
        public IFrameworkServiceContainer CreateContainer(string name = null)
        {
            return new DefaultServiceContainer
            {
                Name = name
            };
        }
    }
}
