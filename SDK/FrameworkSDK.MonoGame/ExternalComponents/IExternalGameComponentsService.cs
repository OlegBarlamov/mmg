using System;
using System.Collections.Generic;
using FrameworkSDK.DependencyInjection;

namespace FrameworkSDK.MonoGame.ExternalComponents
{
    internal interface IExternalGameComponentsService : IDisposable
    {
        void LoadComponents(IServiceLocator serviceLocator);

        IReadOnlyCollection<IExternalGameComponent> GetComponents();
    }
}