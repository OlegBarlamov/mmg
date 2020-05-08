using System;
using System.Collections.Generic;
using FrameworkSDK.IoC;

namespace FrameworkSDK.MonoGame.ExternalComponents
{
    internal class FakeExternalGameComponentsService : IExternalGameComponentsService
    {
        public void Dispose()
        {
        }

        public void RegisterComponent(Type componentType)
        {
        }

        public void AddComponent(IExternalGameComponent component)
        {
        }

        public void RegisterComponents(IServiceRegistrator serviceRegistrator)
        {
        }

        public void ResolveComponents(IServiceLocator serviceLocator)
        {
        }

        public IReadOnlyCollection<IExternalGameComponent> GetComponents()
        {
            return Array.Empty<IExternalGameComponent>();
        }
    }
}