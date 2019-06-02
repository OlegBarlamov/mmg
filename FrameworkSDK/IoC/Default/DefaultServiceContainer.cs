using System;
using System.Collections.Generic;

namespace FrameworkSDK.IoC.Default
{
	internal class DefaultServiceContainer : IFrameworkServiceContainer
	{
		private bool _isDisposed;
		private readonly List<RegistrationInfo> _registrations = new List<RegistrationInfo>();

		public void RegisterInstance<T>(T instance)
		{
			if (instance == null) throw new ArgumentNullException(nameof(instance));

			CheckDisposed();

			var regInfo = RegistrationInfo.FromInstance(instance);
			_registrations.Add(regInfo);
		}

		public void RegisterType<TService, TImpl>(ResolveType resolveType = ResolveType.Singletone)
		{
			CheckDisposed();

			var regInfo = RegistrationInfo.FromType(typeof(TService), typeof(TImpl), resolveType);
			_registrations.Add(regInfo);
		}

		public IServiceLocator BuildContainer()
		{
			CheckDisposed();

			var dependencyResolver = new DefaultDependencyResolver();
			var constructorFinder = new DefaultConstructorFinder();
			var serviceLocator = new DefaultServiceLocator(_registrations, constructorFinder, dependencyResolver);
			//TODO плохо это. Надо сделать инжектирование через функцию
			dependencyResolver.ServiceLocator = serviceLocator;
			constructorFinder.ServiceLocator = serviceLocator;
			return serviceLocator;
		}

		public void Dispose()
		{
			_isDisposed = true;

			_registrations.Clear();
		}

		private void CheckDisposed()
		{
			if (_isDisposed)
				throw new ObjectDisposedException(nameof(DefaultServiceContainer));
		}
	}
}