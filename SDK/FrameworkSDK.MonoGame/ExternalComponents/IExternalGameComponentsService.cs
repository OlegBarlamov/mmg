using System;
using System.Collections.Generic;
using FrameworkSDK.IoC;

namespace FrameworkSDK.MonoGame.ExternalComponents
{
    internal interface IExternalGameComponentsService : IDisposable
    {
        void RegisterComponent(Type componentType);

        void AddComponent(IExternalGameComponent component);

        void RegisterComponents(IServiceRegistrator serviceRegistrator);

        void ResolveComponents(IServiceLocator serviceLocator);

        IReadOnlyCollection<IExternalGameComponent> GetComponents();
    }
}