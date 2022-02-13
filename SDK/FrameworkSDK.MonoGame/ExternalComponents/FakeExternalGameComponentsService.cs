using System;
using System.Collections.Generic;
using FrameworkSDK.DependencyInjection;
using JetBrains.Annotations;

namespace FrameworkSDK.MonoGame.ExternalComponents
{
    [UsedImplicitly]
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

        public void LoadComponents(IServiceLocator serviceLocator)
        {
        }

        public IReadOnlyCollection<IExternalGameComponent> GetComponents()
        {
            return Array.Empty<IExternalGameComponent>();
        }
    }
}