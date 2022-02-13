using System;
using System.Collections.Generic;
using FrameworkSDK.DependencyInjection;
using FrameworkSDK.MonoGame.Core;
using JetBrains.Annotations;

namespace FrameworkSDK.MonoGame.ExternalComponents
{
    internal class ExternalGameComponentsService : IExternalGameComponentsService
    {
        private readonly List<IExternalGameComponent> _components = new List<IExternalGameComponent>();

        public void LoadComponents(IServiceLocator serviceLocator)
        {
            if (serviceLocator.IsServiceRegistered<IExternalGameComponent>())
            {
                var resolvedComponents = serviceLocator.ResolveMultiple<IExternalGameComponent>();
                _components.AddRange(resolvedComponents);
            }
        }

        public IReadOnlyCollection<IExternalGameComponent> GetComponents()
        {
            return _components;
        }

        public void Dispose()
        {
            _components.Clear();
        }
    }
}