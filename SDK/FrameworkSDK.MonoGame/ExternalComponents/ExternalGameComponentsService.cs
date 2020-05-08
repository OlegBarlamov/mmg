using System;
using System.Collections.Generic;
using FrameworkSDK.IoC;
using FrameworkSDK.MonoGame.Core;
using JetBrains.Annotations;

namespace FrameworkSDK.MonoGame.ExternalComponents
{
    internal class ExternalGameComponentsService : IExternalGameComponentsService
    {
        private readonly List<Type> _typesForRegister = new List<Type>();
        private readonly List<IExternalGameComponent> _components = new List<IExternalGameComponent>();

        public void RegisterComponent([NotNull] Type componentType)
        {
            if (componentType == null) throw new ArgumentNullException(nameof(componentType));
            _typesForRegister.Add(componentType);
        }

        public void AddComponent([NotNull] IExternalGameComponent component)
        {
            if (component == null) throw new ArgumentNullException(nameof(component));
            _components.Add(component);
        }

        public void RegisterComponents(IServiceRegistrator serviceRegistrator)
        {
            foreach (var type in _typesForRegister)
            {
                serviceRegistrator.RegisterType(typeof(IExternalGameComponent), type);
            }
        }

        public void ResolveComponents(IServiceLocator serviceLocator)
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
            _typesForRegister.Clear();
            _components.Clear();
        }
    }
}