using System;
using System.Linq;
using FrameworkSDK.Common;
using FrameworkSDK.DependencyInjection.Default.Misc;
using FrameworkSDK.DependencyInjection.Default.Models;
using FrameworkSDK.Localization;
using FrameworkSDK.Logging;
using JetBrains.Annotations;
using NetExtensions.Collections;

namespace FrameworkSDK.DependencyInjection.Default
{
	internal class DefaultServiceLocator : IServiceLocator
	{
        public string Name { get; }

	    private IDisposableExtended LifeTimeScope { get; }
	    private IConstructorFinder ConstructorFinder { get; set; }
		private IDependencyResolver DependencyResolver { get; set; }
		private IServicesRegistrationsPriority ServicesRegistrationsPriority { get; set; }
		private IServicesCandidatesFinder ServicesCandidatesFinder { get; set; }
		
		private bool _isInitialized;
		
		private readonly string _id = GetId();
	    private readonly ModuleLogger _logger;
	    private readonly RegisteredServicesMapping _mapping;

	    public DefaultServiceLocator(
			[NotNull] IFrameworkLogger frameworkLogger,
			[NotNull] IDisposableExtended lifeTimeScope,
			[NotNull] RegistrationsDomain registrationsDomain,
			[CanBeNull] string name = null)
		{
			if (frameworkLogger == null) throw new ArgumentNullException(nameof(frameworkLogger));
			if (registrationsDomain == null) throw new ArgumentNullException(nameof(registrationsDomain));
		    LifeTimeScope = lifeTimeScope ?? throw new ArgumentNullException(nameof(lifeTimeScope));
		    Name = name;
		    
		    _logger = new ModuleLogger(frameworkLogger, LogCategories.Application);
		    _mapping = new RegisteredServicesMapping(registrationsDomain.GetAll());

		    LifeTimeScope.DisposedEvent += LifeTimeScopeOnDisposedEvent;
		    LogServiceLocatorCreated();
		}

	    private void LogServiceLocatorCreated()
	    {
		    _logger.Log($"{ToString()} created with mapping: " + Environment.NewLine + _mapping, FrameworkLogLevel.Trace);
	    }

	    public void InitializeInternalServices(
		    [NotNull] IConstructorFinder constructorFinder,
		    [NotNull] IDependencyResolver dependencyResolver,
		    [NotNull] IServicesRegistrationsPriority servicesRegistrationsPriority,
		    [NotNull] IServicesCandidatesFinder servicesCandidatesFinder)
	    {
		    ConstructorFinder = constructorFinder ?? throw new ArgumentNullException(nameof(constructorFinder));
		    DependencyResolver = dependencyResolver ?? throw new ArgumentNullException(nameof(dependencyResolver));
		    ServicesRegistrationsPriority = servicesRegistrationsPriority ?? throw new ArgumentNullException(nameof(servicesRegistrationsPriority));
		    ServicesCandidatesFinder = servicesCandidatesFinder ?? throw new ArgumentNullException(nameof(servicesCandidatesFinder));

		    _isInitialized = true;
	    }

	    public override string ToString()
	    {
	        var name = string.IsNullOrWhiteSpace(Name)
	            ? $"{nameof(DefaultServiceLocator)}_{_id}"
	            : Name;

            return name;
	    }

	    public object Resolve(Type type, object[] parameters = null)
		{
			if (type == null) throw new ArgumentNullException(nameof(type));
			CheckDisposed();
			CheckInitialized();
			
			var candidates = ServicesCandidatesFinder.FindCandidates(type, _mapping);
			if (candidates.Count < 1)
				throw new FrameworkIocException(Strings.Exceptions.Ioc.TypeNotRegisteredException, type, this);
			
			var preferableRegistration = ServicesRegistrationsPriority.GetPreferable(type, candidates);
			return preferableRegistration.GetOrSet(type, this, parameters ?? Array.Empty<object>());
		}

	    public Array ResolveMultiple(Type type)
	    {
		    if (type == null) throw new ArgumentNullException(nameof(type));
		    CheckDisposed();
		    CheckInitialized();

		    var candidates = ServicesCandidatesFinder.FindCandidates(type, _mapping).ToArray();
		    var instancesArray = Array.CreateInstance(type, candidates.Length);
		    candidates.For((candidate, i) =>
		    {
			    instancesArray.SetValue(candidate.GetOrSet(type, this, Array.Empty<object>()), i);
			    return false;
		    });
		    
		    return instancesArray;
	    }

	    public bool IsServiceRegistered(Type type)
		{
			if (type == null) throw new ArgumentNullException(nameof(type));
			CheckDisposed();
			CheckInitialized();

			var candidates = ServicesCandidatesFinder.FindCandidates(type, _mapping);
			return candidates.Count > 0;
		}

		public object CreateInstance(Type type, object[] parameters = null)
	    {
	        try
	        {
		        return CreateInstanceInternal(type, parameters ?? Array.Empty<object>());
	        }
	        catch (Exception e)
	        {
	            throw new FrameworkIocException(Strings.Exceptions.Ioc.ResolvingTypeException, e, type);
	        }
	    }

        private object CreateInstanceInternal(Type type, object[] parameters)
        {
	        var parametersTypes = parameters.Select(o => o.GetType()).ToArray();
	        var constructor = ConstructorFinder.GetConstructor(this, type, parametersTypes);
	        var dependencies = DependencyResolver.ResolveDependencies(this, constructor, parameters);
	        var instance = constructor.Invoke(dependencies);
	        return instance;
        }

        private void CheckDisposed()
		{
			if (LifeTimeScope.IsDisposed)
				throw new ObjectDisposedException(LifeTimeScope.ToString());
		}

		private void CheckInitialized()
		{
			if (!_isInitialized)
				throw new InvalidOperationException($"{Name} must be initialized");
		}

	    private void LifeTimeScopeOnDisposedEvent(object sender, EventArgs args)
	    {
	        LifeTimeScope.DisposedEvent -= LifeTimeScopeOnDisposedEvent;
	        _logger.Trace($"{ToString()} disposed");
	        _logger.Dispose();
	    }

	    private static string GetId()
	    {
	        var guid = Guid.NewGuid().ToString("D");
	        return guid.Split('-')[1];
	    }
    }
}