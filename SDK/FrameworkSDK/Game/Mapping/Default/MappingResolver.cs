using System;
using System.Collections.Generic;
using System.Linq;
using FrameworkSDK.IoC;
using FrameworkSDK.Localization;
using JetBrains.Annotations;
using NetExtensions;

namespace FrameworkSDK.Game.Mapping.Default
{
	internal class MappingResolver<T> : IDisposable
	{
		private string ModelTitle { get; }
		private string TypeTitle { get; }

		private IServiceLocator MappingLocator => _mappingLocator ?? CreateLocator();

		private readonly IFrameworkServiceContainer _mappingContainer;
		private readonly Dictionary<int, Type> _mappingDictionary = new Dictionary<int, Type>();

		private IServiceLocator _mappingLocator;
		private bool _disposed;

		public MappingResolver([NotNull] IFrameworkServiceContainer serviceContainer,
			[NotNull] string typeTitle, [NotNull] string modelTitle)
		{
			ModelTitle = modelTitle ?? throw new ArgumentNullException(nameof(modelTitle));
			TypeTitle = typeTitle ?? throw new ArgumentNullException(nameof(typeTitle));
			if (serviceContainer == null) throw new ArgumentNullException(nameof(serviceContainer));
			if (string.IsNullOrWhiteSpace(typeTitle)) throw new ArgumentException(nameof(typeTitle));
			if (string.IsNullOrWhiteSpace(modelTitle)) throw new ArgumentException(nameof(modelTitle));

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

			var hash = GetModelHash(model, out var modelName);
			var type = ResolveTypeByModelHash(hash, modelName);

			return (T)ResolveWithParameter(type, model);
		}

		public bool IsModelHasMapping(object model)
		{
			if (_disposed) throw new ObjectDisposedException(nameof(MappingResolver<T>));
			var hash = GetModelHash(model, out _);
			return _mappingDictionary.ContainsKey(hash);
		}

		private IServiceLocator CreateLocator()
		{
			_mappingLocator = _mappingContainer.BuildContainer();
			return _mappingLocator;
		}

		private Type ResolveTypeByModelHash(int modelHash, string modelTypeName)
		{
			if (!_mappingDictionary.ContainsKey(modelHash))
				throw new MappingException(string.Format(Strings.Exceptions.Mapping.ControllerForModelNotFound,
					modelTypeName));

			return _mappingDictionary[modelHash];
		}

		private object ResolveWithParameter([NotNull] Type type, [NotNull] object parameter)
		{
			if (type == null) throw new ArgumentNullException(nameof(type));
			if (parameter == null) throw new ArgumentNullException(nameof(parameter));
			return MappingLocator.ResolveWithParameters(type, new[] { parameter });
		}

		private int GetModelHash([NotNull] object model, out string modelName)
		{
			if (model == null) throw new ArgumentNullException(nameof(model));

			var type = model.GetType();
			modelName = type.Name;
			if (modelName.EndsWith(ModelTitle, StringComparison.InvariantCultureIgnoreCase))
				modelName = modelName.TrimEnd(ModelTitle);

			return modelName.GetHashCode();
		}
	}
}
