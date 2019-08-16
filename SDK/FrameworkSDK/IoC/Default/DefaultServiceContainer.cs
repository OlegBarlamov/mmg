using System;
using System.Collections.Generic;

namespace FrameworkSDK.IoC.Default
{
	internal class DefaultServiceContainer : IFrameworkServiceContainer
	{
		private bool _isDisposed;
		private readonly List<RegistrationInfo> _registrations;

		public DefaultServiceContainer()
			:this(new RegistrationInfo[0])
		{
		}

		private DefaultServiceContainer(IEnumerable<RegistrationInfo> registrations)
		{
			_registrations = new List<RegistrationInfo>(registrations);
		}

	    public void RegisterInstance(Type serviceType, object instance)
	    {
	        if (instance == null) throw new ArgumentNullException(nameof(instance));

	        CheckDisposed();

	        var regInfo = RegistrationInfo.FromInstance(serviceType, instance);
	        _registrations.Add(regInfo);
        }

	    public void RegisterType(Type serviceType, Type implType, ResolveType resolveType = ResolveType.Singletone)
	    {
	        CheckDisposed();

	        var regInfo = RegistrationInfo.FromType(serviceType, implType, resolveType);
	        _registrations.Add(regInfo);
        }

		public IServiceLocator BuildContainer()
		{
			CheckDisposed();

			return new DefaultServiceLocator(_registrations);
		}

		public IFrameworkServiceContainer Clone()
		{
			return new DefaultServiceContainer(_registrations);
		}

		public void Dispose()
		{
			_isDisposed = true;

		    foreach (var registrationInfo in _registrations)
		        registrationInfo.Dispose();

			_registrations.Clear();
		}

		private void CheckDisposed()
		{
			if (_isDisposed)
				throw new ObjectDisposedException(nameof(DefaultServiceContainer));
		}
	}
}