using System;
using System.Collections.Generic;
using System.Linq;
using FrameworkSDK.Common;
using FrameworkSDK.DependencyInjection.Default.Models;
using FrameworkSDK.Localization;
using FrameworkSDK.Logging;
using JetBrains.Annotations;

namespace FrameworkSDK.DependencyInjection.Default
{
	public class DefaultServiceContainer : IFrameworkServiceContainer, IDisposableExtended
	{
		public event EventHandler DisposedEvent;
		public string Name { get; set; }
		
		[CanBeNull] private DefaultServiceLocatorFactory DefaultServiceLocatorFactory { get; set; }
		[NotNull] private IFrameworkLogger Logger { get; set; }
		[NotNull] private RegistrationsDomain RegistrationsDomain { get; }
	    
	    private bool _isDisposed;
	    
		bool IDisposableExtended.IsDisposed => _isDisposed;

		public DefaultServiceContainer(IFrameworkLogger logger, [NotNull] RegistrationsDomain registrationsDomain)
		{
			Logger = logger;
			RegistrationsDomain = registrationsDomain ?? throw new ArgumentNullException(nameof(registrationsDomain));
		}
		
		private DefaultServiceContainer([NotNull] DefaultServiceLocatorFactory defaultServiceLocatorFactory, RegistrationsDomain registrationsDomain)
			:this(AppContext.Logger, registrationsDomain)
		{
			DefaultServiceLocatorFactory = defaultServiceLocatorFactory ?? throw new ArgumentNullException(nameof(defaultServiceLocatorFactory));
		}

		public void RegisterInstance(Type serviceType, object instance)
	    {
	        if (instance == null) throw new ArgumentNullException(nameof(instance));
	        CheckDisposed();

	        var regInfo = new InstanceRegistrationInfo(serviceType, instance);
	        RegistrationsDomain.Add(regInfo);
        }

	    public void RegisterType(Type serviceType, Type implType, ResolveType resolveType = ResolveType.Singletone)
	    {
	        CheckDisposed();

	        var regInfo = new TypeRegistrationInfo(serviceType, implType, resolveType);
	        RegistrationsDomain.Add(regInfo);
        }

	    public void RegisterGeneric(Type genericServiceTypeDefinition, Type genericImplementationTypeDefinition,
		    ResolveType resolveType = ResolveType.Singletone)
	    {
		    CheckDisposed();
		    
		    var regInfo = new GenericRegistrationInfo(genericServiceTypeDefinition, genericImplementationTypeDefinition, resolveType);
		    RegistrationsDomain.Add(regInfo);
	    }

	    public void RegisterGenericFactory(Type genericServiceTypeDefinition, ServiceFactoryDelegate factory,
		    ResolveType resolveType = ResolveType.Singletone)
	    {
		    CheckDisposed();
		    
		    var regInfo = new GenericFactoryRegistrationInfo(genericServiceTypeDefinition, factory, resolveType);
		    RegistrationsDomain.Add(regInfo);
	    }

	    public void RegisterFactory(Type serviceType, ServiceFactoryDelegate factory, ResolveType resolveType)
	    {
		    CheckDisposed();
		    
		    var regInfo = new FactoryRegistrationInfo(serviceType, factory, resolveType);
		    RegistrationsDomain.Add(regInfo);
	    }

	    public IServiceLocator BuildContainer()
		{
			CheckDisposed();

			if (DefaultServiceLocatorFactory == null)
			{
				var firstServiceLocator = DefaultServiceLocatorFactory.CreateFirstDefaultServiceLocator(new NullLogger(), RegistrationsDomain, this, "FirstLocator");
				DefaultServiceLocatorFactory = firstServiceLocator.Resolve<DefaultServiceLocatorFactory>();
			}

			return DefaultServiceLocatorFactory.Create(RegistrationsDomain, this, Name);
		}

		public IFrameworkServiceContainer CreateScoped(string name = null)
		{
			CheckDisposed();
			if (DefaultServiceLocatorFactory == null) throw new InvalidOperationException(nameof(DefaultServiceLocatorFactory) + " should be initialized");
			return new DefaultServiceContainer(DefaultServiceLocatorFactory, RegistrationsDomain.CreateScoped())
			{
                Name = name
            };
		}

		public bool ContainsRegistrationForType(Type type)
		{
			return RegistrationsDomain.ContainsRegistrationForType(type);
		}

		public void Dispose()
		{
			CheckDisposed();
			_isDisposed = true;

		    var allDisposable = RegistrationsDomain.GetCurrent().SelectMany(list => list.GetDisposableCashedObjects());
		    var exceptions = new List<Exception>();
		    foreach (var disposable in allDisposable)
		    {
		        try
		        {
		            disposable.Dispose();
		        }
		        catch (Exception e)
		        {
		            exceptions.Add(e);
		        }
		    }

		    DisposedEvent?.Invoke(this, EventArgs.Empty);

            if (exceptions.Count > 0)
		        throw new AggregateException(Strings.Exceptions.Ioc.DisposeServicesException, exceptions);
        }

		private void CheckDisposed()
		{
			if (_isDisposed)
				throw new ObjectDisposedException(nameof(DefaultServiceContainer));
		}
	}
}