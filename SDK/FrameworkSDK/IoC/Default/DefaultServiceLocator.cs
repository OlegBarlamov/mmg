using System;
using System.Collections.Generic;
using System.Linq;
using FrameworkSDK.Common;
using FrameworkSDK.Localization;
using FrameworkSDK.Logging;
using JetBrains.Annotations;

namespace FrameworkSDK.IoC.Default
{
	internal class DefaultServiceLocator : IServiceLocator
	{
        public string Name { get; set; }

	    private IDisposableExtended LifeTimeScope { get; }
	    private ConstructorFinder ConstructorFinder { get; }
		private DependencyResolver DependencyResolver { get; }

		private readonly Dictionary<int, List<RegistrationInfo>> _mapping = new Dictionary<int, List<RegistrationInfo>>();

	    private readonly string _id = GetId();
	    private readonly AutoDefferedLogger _defferedLogger;
	    private readonly ModuleLogger _logger;

		public DefaultServiceLocator([NotNull] IDisposableExtended lifeTimeScope, [NotNull, ItemNotNull] IReadOnlyCollection<RegistrationInfo> registrations, [CanBeNull] string name = null)
		{
			if (registrations == null) throw new ArgumentNullException(nameof(registrations));
		    LifeTimeScope = lifeTimeScope ?? throw new ArgumentNullException(nameof(lifeTimeScope));

		    ConstructorFinder = new ConstructorFinder(this);
			DependencyResolver = new DependencyResolver(this);

			foreach (var registrationInfo in registrations)
			{
				var code = GetCode(registrationInfo);
				if (!_mapping.ContainsKey(code))
					_mapping.Add(code, new List<RegistrationInfo>());

				_mapping[code].Add(registrationInfo);
			}

		    LifeTimeScope.DisposedEvent += LifeTimeScopeOnDisposedEvent;

		    _defferedLogger = new AutoDefferedLogger(AppContext.FindLogger);
		    _logger = new ModuleLogger(_defferedLogger, FrameworkLogModule.Application);

		    Name = name;
            LogCreationInfo(registrations);
        }

	    public override string ToString()
	    {
	        var name = string.IsNullOrWhiteSpace(Name)
	            ? $"{nameof(DefaultServiceLocator)}_{_id}"
	            : Name;

            return name;
	    }

	    public object Resolve(Type type)
		{
			if (type == null) throw new ArgumentNullException(nameof(type));
			CheckDisposed();

			var regInfos = GetRegInfos(type);
			var lastReg = regInfos.Last();

			return ResolveRegInfo(lastReg);
		}

	    public object ResolveWithParameters(Type type, [NotNull] object[] parameters)
	    {
            if (type == null) throw new ArgumentNullException(nameof(type));
	        if (parameters == null) throw new ArgumentNullException(nameof(parameters));
	        CheckDisposed();

	        var regInfos = GetRegInfos(type);
	        var lastReg = regInfos.Last();
		    if (lastReg.ResolveType == ResolveType.Singletone)
			    throw new FrameworkIocException(Strings.Exceptions.Ioc.BadResolveStrategy, type.Name);

			return ResolveRegInfo(lastReg, parameters);
	    }

		public IReadOnlyList<object> ResolveMultiple(Type type)
		{
			if (type == null) throw new ArgumentNullException(nameof(type));
			CheckDisposed();

			var regInfos = GetRegInfos(type);
			var result = regInfos.Select(ResolveRegInfo);
			return result.ToArray();
		}

		public bool IsServiceRegistered(Type type)
		{
			if (type == null) throw new ArgumentNullException(nameof(type));
			CheckDisposed();

			var code = GetCode(type);
			return _mapping.ContainsKey(code);
		}

	    private object ResolveRegInfo(RegistrationInfo regInfo)
	    {
	        return ResolveRegInfo(regInfo, Array.Empty<object>());
	    }

        private object ResolveRegInfo(RegistrationInfo regInfo, object[] parameters)
		{
			if (regInfo.CashedInstance != null && regInfo.ResolveType == ResolveType.Singletone)
				return regInfo.CashedInstance;

		    var instance = parameters.Length == 0
		        ? CreateInstance(regInfo.ImplType)
		        : CreateInstanceWithParameters(regInfo.ImplType, parameters);

            LogCreatedInstance(regInfo.Type, instance);

			if (regInfo.ResolveType == ResolveType.Singletone)
				regInfo.CashedInstance = instance;

			return instance;
		}

		private IReadOnlyList<RegistrationInfo> GetRegInfos(Type type)
		{
			var code = GetCode(type);
			if (!_mapping.TryGetValue(code, out var regInfos) || regInfos.Count < 1)
				throw new FrameworkIocException(Strings.Exceptions.Ioc.TypeNotRegisteredException, type);

			return regInfos;
		}

	    private object CreateInstanceWithParameters(Type type, object[] parameters)
	    {
	        try
	        {
		        var parametersTypes = parameters.Select(o => o.GetType()).ToArray();
	            var constructor = ConstructorFinder.GetConstructorWithParameters(type, parametersTypes);
	            var dependencies = DependencyResolver.ResolveDependenciesWithParameters(constructor, parameters);
	            return constructor.Invoke(dependencies);
	        }
	        catch (Exception e)
	        {
	            throw new FrameworkIocException(Strings.Exceptions.Ioc.ResolvingTypeException, e, type);
	        }
	    }

        private object CreateInstance(Type type)
		{
			try
			{
				var constructor = ConstructorFinder.GetConstructor(type);
				var dependencies = DependencyResolver.ResolveDependencies(constructor);
				return constructor.Invoke(dependencies);
			}
			catch (Exception e)
			{
				throw new FrameworkIocException(Strings.Exceptions.Ioc.ResolvingTypeException, e, type);
			}
		}

		private static int GetCode([NotNull] Type type)
		{
			if (type == null) throw new ArgumentNullException(nameof(type));

			return type.GetHashCode();
		}

		private static int GetCode([NotNull] RegistrationInfo registrationInfo)
		{
			if (registrationInfo == null) throw new ArgumentNullException(nameof(registrationInfo));

			var type = registrationInfo.Type;
			return GetCode(type);
		}

		private void CheckDisposed()
		{
			if (LifeTimeScope.IsDisposed)
				throw new ObjectDisposedException(LifeTimeScope.ToString());
		}

	    private void LifeTimeScopeOnDisposedEvent()
	    {
	        LifeTimeScope.DisposedEvent -= LifeTimeScopeOnDisposedEvent;
            _mapping.Clear();

	        _logger.Dispose();
            _defferedLogger.Dispose();
        }

	    private void LogCreationInfo(IReadOnlyCollection<RegistrationInfo> registrations)
	    {
	        var registrationsInfo = Environment.NewLine + string.Join(Environment.NewLine, registrations.Select(info => "  " + info));
            _logger.Debug(Strings.Info.AppConstructing.ServiceLocatorCreated, this, registrationsInfo);
        }

	    private void LogCreatedInstance(Type sourceType, object instance)
	    {
	        _logger.Debug(Strings.Info.AppConstructing.ServiceLocatorInstanceCreated, this, $"{sourceType.Name}->{instance?.GetTypeName()}");
        }

        private static string GetId()
	    {
	        var guid = Guid.NewGuid().ToString("D");
	        return guid.Split('-')[1];
	    }
    }
}