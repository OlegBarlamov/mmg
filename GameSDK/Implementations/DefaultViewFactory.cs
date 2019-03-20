using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Autofac;
using GameSDK.Services;
using JetBrains.Annotations;
using Logging;
using Microsoft.Extensions.Logging;

namespace GameSDK.Implementations
{
	public class DefaultViewFactory : IViewFactory
	{
		private IContainer ServiceLocator { get; }
		private ILogger Logger { get; }

		public DefaultViewFactory([NotNull] IContainer serviceLocator, [NotNull] ILoggerFactory loggerFactory)
		{
			if (loggerFactory == null) throw new ArgumentNullException(nameof(loggerFactory));
			ServiceLocator = serviceLocator ?? throw new ArgumentNullException(nameof(serviceLocator));
			Logger = loggerFactory.CreateLogger("ViewFactoring");
		}

		public T CreateView<T>(object model) where T : IView
		{
			var createdType = typeof(T);
			var modelType = model?.GetType();

			Logger.Info($"Creating view {createdType.Name}");

			var constructors = createdType.GetConstructors();
			Logger.Info($"Founding {constructors.Length} constructors");

			var suitableConstructor = constructors.FirstOrDefault(info => IsConstructorSuitable(info, modelType));
			if (suitableConstructor == null)
			{
				var message = $"Can not found suitable constructors for type {createdType} and model {model}";
				Logger.Error(message);
				throw new GameSDKException(message);
			}

			return ExecuteConstructor<T>(suitableConstructor, model, modelType);
		}

		private bool IsConstructorSuitable(ConstructorInfo constructorInfo, Type modelType)
		{
			var parameters = constructorInfo.GetParameters();
			var parametersTypes = parameters.Select(info => info.ParameterType).ToList();
			if (modelType != null)
			{
				var modelParameter = GetModelParameter(parametersTypes, modelType);
				if (modelParameter == null)
					return false;

				parametersTypes.Remove(modelParameter);
			}

			return parametersTypes.All(type => ServiceLocator.IsRegistered(type));
		}

		private T ExecuteConstructor<T>(ConstructorInfo constructorInfo, object model, Type modelType)
		{
			var parameters = constructorInfo.GetParameters();
			var parametersTypes = parameters.Select(info => info.ParameterType).ToList();
			var inputParameters = new object[parametersTypes.Count];
			if (model != null)
			{
				var modelParameter = GetModelParameter(parametersTypes, modelType);
				var index = parametersTypes.IndexOf(modelParameter);
				inputParameters[index] = model;
			}

			for (int i = 0; i < inputParameters.Length; i++)
			{
				if (inputParameters[i] != null)
					continue;

				inputParameters[i] = ServiceLocator.Resolve(parametersTypes[i]);
			}

			return (T)constructorInfo.Invoke(inputParameters);
		}

		private Type GetModelParameter(IEnumerable<Type> parametersTypes, Type modelType)
		{
			return parametersTypes.FirstOrDefault(type => type.IsAssignableFrom(modelType));
		}
	}
}
 