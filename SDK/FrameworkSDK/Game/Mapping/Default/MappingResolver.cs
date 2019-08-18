using System;
using System.Collections.Generic;
using System.Linq;
using FrameworkSDK.Game.Controllers;
using FrameworkSDK.Game.Views;
using FrameworkSDK.IoC;
using FrameworkSDK.Localization;
using JetBrains.Annotations;
using NetExtensions;

namespace FrameworkSDK.Game.Mapping.Default
{
	internal class MappingResolver<T> : IDisposable
	{
		private static string ModelTitle = "Model";
		private static string ViewTitle = nameof(View);
		private static string ControllerTitle = nameof(Controller);

		private string TypeTitle { get; }

		private IServiceLocator MappingLocator => _mappingLocator ?? CreateLocator();

		private readonly IFrameworkServiceContainer _mappingContainer;
		private readonly Dictionary<int, Type> _mappingDictionary = new Dictionary<int, Type>();

		private IServiceLocator _mappingLocator;
		private bool _disposed;

		public MappingResolver([NotNull] IFrameworkServiceContainer serviceContainer,
			[NotNull] string typeTitle)
		{
			TypeTitle = typeTitle ?? throw new ArgumentNullException(nameof(typeTitle));
			if (serviceContainer == null) throw new ArgumentNullException(nameof(serviceContainer));

			_mappingContainer = serviceContainer.Clone();
		}

		public void RegisterTypes([NotNull, ItemNotNull] IEnumerable<Type> registeredTypes)
		{
			if (registeredTypes == null) throw new ArgumentNullException(nameof(registeredTypes));
			if (_disposed) throw new ObjectDisposedException(nameof(MappingResolver<T>));

			var targetTypes = registeredTypes.Where(type => typeof(T).IsAssignableFrom(type) &&
			                                                type.Name.EndsWith(TypeTitle, StringComparison.InvariantCultureIgnoreCase));

			foreach (var type in targetTypes)
			{
				var modelName = type.Name.TrimEnd(TypeTitle);
				var modelHash = modelName.GetHashCode();
				_mappingContainer.RegisterType(type, type, ResolveType.InstancePerResolve);
				_mappingDictionary.Add(modelHash, type);
			}
		}

		public void Dispose()
		{
			_disposed = true;
			_mappingDictionary.Clear();
			_mappingLocator?.Dispose();
			_mappingContainer.Dispose();
		}

		public T ResolveByModel(object model)
		{
			if (_disposed) throw new ObjectDisposedException(nameof(MappingResolver<T>));
			if (model == null) throw new ArgumentNullException(nameof(model));
			return Resolve(model, ModelTitle);
		}

		public bool IsModelHasMapping(object model)
		{
			if (_disposed) throw new ObjectDisposedException(nameof(MappingResolver<T>));
			return IsHasMapping(model, ModelTitle);
		}

		public T ResolveByView([NotNull] IView view)
		{
			if (view == null) throw new ArgumentNullException(nameof(view));
			if (_disposed) throw new ObjectDisposedException(nameof(MappingResolver<T>));
			return Resolve(view, ViewTitle);
		}

		public bool IsViewHasMapping([NotNull] IView view)
		{
			if (view == null) throw new ArgumentNullException(nameof(view));
			if (_disposed) throw new ObjectDisposedException(nameof(MappingResolver<T>));
			return IsHasMapping(view, ViewTitle);
		}

		public T ResolveByController([NotNull] IController controller)
		{
			if (controller == null) throw new ArgumentNullException(nameof(controller));
			if (_disposed) throw new ObjectDisposedException(nameof(MappingResolver<T>));
			return Resolve(controller, ControllerTitle);
		}

		public bool IsControllerHasMapping([NotNull] IController controller)
		{
			if (controller == null) throw new ArgumentNullException(nameof(controller));
			if (_disposed) throw new ObjectDisposedException(nameof(MappingResolver<T>));
			return IsHasMapping(controller, ControllerTitle);
		}

		private T Resolve(object instance, string instanceTitle)
		{
			var hash = GetTypeHash(instance, instanceTitle, out var instanceName);
			var type = ResolveTypeByHash(hash, instanceName);

			return (T)ResolveWithParameter(type, instance);
		}

		private bool IsHasMapping(object instance, string instanceTitle)
		{
			var hash = GetTypeHash(instance, instanceTitle, out _);
			return _mappingDictionary.ContainsKey(hash);
		}

		private IServiceLocator CreateLocator()
		{
			_mappingLocator = _mappingContainer.BuildContainer();
			return _mappingLocator;
		}

		private Type ResolveTypeByHash(int hash, string typeName)
		{
			if (!_mappingDictionary.ContainsKey(hash))
				throw new MappingException(Strings.Exceptions.Mapping.MappingForInstanceNotFound, typeName);

			return _mappingDictionary[hash];
		}

		private object ResolveWithParameter([NotNull] Type type, [NotNull] object parameter)
		{
			if (type == null) throw new ArgumentNullException(nameof(type));
			if (parameter == null) throw new ArgumentNullException(nameof(parameter));
			return MappingLocator.ResolveWithParameters(type, new[] { parameter });
		}

		private int GetTypeHash([NotNull] object model, string typeNamePattern, out string typeName)
		{
			if (model == null) throw new ArgumentNullException(nameof(model));

			var type = model.GetType();
			typeName = type.Name;
			if (typeName.EndsWith(typeNamePattern, StringComparison.InvariantCultureIgnoreCase))
				typeName = typeName.TrimEnd(typeNamePattern);

			return typeName.GetHashCode();
		}
	}
}
