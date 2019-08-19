using System;
using System.Collections.Generic;
using System.Linq;
using FrameworkSDK.Common;
using FrameworkSDK.Localization;

namespace FrameworkSDK.IoC.Default
{
	internal class DefaultServiceContainer : IFrameworkServiceContainer, IDisposableExtended
	{
	    public event Action DisposedEvent;

        private bool _isDisposed;

		private readonly List<RegistrationInfo> _parentRegistrations;

        private readonly List<RegistrationInfo> _myRegistrations = new List<RegistrationInfo>();

	    bool IDisposableExtended.IsDisposed => _isDisposed;

        public DefaultServiceContainer()
			:this(new RegistrationInfo[0])
		{
		}

		private DefaultServiceContainer(IEnumerable<RegistrationInfo> registrations)
		{
		    _parentRegistrations = new List<RegistrationInfo>(registrations);
		}

	    public void RegisterInstance(Type serviceType, object instance)
	    {
	        if (instance == null) throw new ArgumentNullException(nameof(instance));

	        CheckDisposed();

	        var regInfo = RegistrationInfo.FromInstance(serviceType, instance);
	        _myRegistrations.Add(regInfo);
        }

	    public void RegisterType(Type serviceType, Type implType, ResolveType resolveType = ResolveType.Singletone)
	    {
	        CheckDisposed();

	        var regInfo = RegistrationInfo.FromType(serviceType, implType, resolveType);
	        _myRegistrations.Add(regInfo);
        }

		public IServiceLocator BuildContainer()
		{
			CheckDisposed();

			return new DefaultServiceLocator(this, GetAllRegistrations());
		}

		public IFrameworkServiceContainer CreateScoped()
		{
			return new DefaultServiceContainer(GetAllRegistrations());
		}

		public void Dispose()
		{
			_isDisposed = true;

		    var allDisposable = FindAllDisposableSingletones();
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

            foreach (var registrationInfo in _myRegistrations)
		        registrationInfo.Dispose();

		    _myRegistrations.Clear();
            _parentRegistrations.Clear();

		    DisposedEvent?.Invoke();

            if (exceptions.Count > 0)
		        throw new AggregateException(Strings.Exceptions.Ioc.DisposeServicesException, exceptions);
        }

		private void CheckDisposed()
		{
			if (_isDisposed)
				throw new ObjectDisposedException(nameof(DefaultServiceContainer));
		}

	    private IReadOnlyCollection<RegistrationInfo> GetAllRegistrations()
	    {
            return new List<RegistrationInfo>(_parentRegistrations.Concat(_myRegistrations));
	    }

	    private IEnumerable<IDisposable> FindAllDisposableSingletones()
	    {
	        var singletonesRegInfos = _myRegistrations
	            .Where(info => info.ResolveType == ResolveType.Singletone);

	        var disposableCashedObjects = singletonesRegInfos
	            .Where(info => info.CashedInstance is IDisposable)
	            .Cast<IDisposable>();

	        return disposableCashedObjects;
	    }
	}
}