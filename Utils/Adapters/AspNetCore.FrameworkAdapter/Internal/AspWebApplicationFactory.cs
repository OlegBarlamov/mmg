using System;
using System.Collections;
using System.Collections.Generic;
using AspNetCore.FrameworkAdapter.Internal;
using FrameworkSDK;
using FrameworkSDK.DependencyInjection;
using FrameworkSDK.Localization;
using FrameworkSDK.Logging;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace AspNetCore.FrameworkAdapter
{
    internal class AspWebApplicationFactory : IAspWebApplicationFactory
    {
        private IServiceCollection ServiceCollection => WebApplicationBuilder.Services;
        private DefaultAppFactory DefaultAppFactory { get; }
        private WebApplicationBuilder WebApplicationBuilder { get; }

        public AspWebApplicationFactory([NotNull] DefaultAppFactory defaultAppFactory, [NotNull] WebApplicationBuilder webApplicationBuilder)
        {
            DefaultAppFactory = defaultAppFactory ?? throw new ArgumentNullException(nameof(defaultAppFactory));
            WebApplicationBuilder = webApplicationBuilder ?? throw new ArgumentNullException(nameof(webApplicationBuilder));
        }

        public IApp Construct()
        {
            var adapter = new FrameworkServiceContainerAdapter(WebApplicationBuilder);
            DefaultAppFactory.UseServiceContainer(adapter);
            var deferrerLogger = new ManualDeferredLogger();
            DefaultAppFactory.UseLogger(deferrerLogger);
            
            var app = DefaultAppFactory.Construct();
            var finalLogger = new FrameworkLoggerAdapter(adapter.WebApplication.Services.GetRequiredService<ILoggerFactory>());
            deferrerLogger.LogTo(finalLogger);
            deferrerLogger.Dispose();
            DefaultAppFactory.UseLogger(finalLogger);
            return new AspWebApp(adapter.WebApplication, app);
        }

        public IAppFactory AddServices(IServicesModule module)
        {
            DefaultAppFactory.AddServices(module);
            return this;
        }

        public IAppFactory AddComponent<TComponent>() where TComponent : class, IAppComponent
        {
            DefaultAppFactory.AddComponent<TComponent>();
            return this;
        }

        public IAppFactory AddComponent(IAppComponent appComponent)
        {
            DefaultAppFactory.AddComponent(appComponent);
            return this;
        }

        public IAspWebApplicationFactory UseLocalization(ILocalization localization)
        {
            DefaultAppFactory.UseLocalization(localization);
            return this;
        }

        public IEnumerator<ServiceDescriptor> GetEnumerator()
        {
            return ServiceCollection.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable) ServiceCollection).GetEnumerator();
        }

        public void Add(ServiceDescriptor item)
        {
            ServiceCollection.Add(item);
        }

        public void Clear()
        {
            ServiceCollection.Clear();
        }

        public bool Contains(ServiceDescriptor item)
        {
            return ServiceCollection.Contains(item);
        }

        public void CopyTo(ServiceDescriptor[] array, int arrayIndex)
        {
            ServiceCollection.CopyTo(array, arrayIndex);
        }

        public bool Remove(ServiceDescriptor item)
        {
            return ServiceCollection.Remove(item);
        }

        public int Count => ServiceCollection.Count;

        public bool IsReadOnly => ServiceCollection.IsReadOnly;

        public int IndexOf(ServiceDescriptor item)
        {
            return ServiceCollection.IndexOf(item);
        }

        public void Insert(int index, ServiceDescriptor item)
        {
            ServiceCollection.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            ServiceCollection.RemoveAt(index);
        }

        public ServiceDescriptor this[int index]
        {
            get => ServiceCollection[index];
            set => ServiceCollection[index] = value;
        }
    }
}