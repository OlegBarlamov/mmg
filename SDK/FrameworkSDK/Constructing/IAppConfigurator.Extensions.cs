using System;
using FrameworkSDK.Pipelines;
using FrameworkSDK.IoC;
using FrameworkSDK.Localization;
using FrameworkSDK.Logging;
using JetBrains.Annotations;
using NetExtensions;

namespace FrameworkSDK.Constructing
{
	public static partial class AppConfiguratorExtensions
	{
		public static IAppConfigurator SetupCustomLocalization([NotNull] this IAppConfigurator configurator, [NotNull] Func<ILocalization> localizationFactory)
		{
			return configurator.SetupCustomLocalization<object>(null, context => localizationFactory());
		}
		
		public static IAppConfigurator SetupCustomLocalization<T>([NotNull] this IAppConfigurator configurator, T context, [NotNull] Func<T, ILocalization> localizationFactory)
		{
			if (configurator == null) throw new ArgumentNullException(nameof(configurator));
			if (localizationFactory == null) throw new ArgumentNullException(nameof(localizationFactory));

			var initializationPhase = configurator.GetStep(DefaultConfigurationSteps.Initialization);
			initializationPhase.AddOrReplace(new SimplePipelineAction(
				DefaultConfigurationSteps.InitializationActions.Localization,
				true,
				ctx =>
				{
					var localization = GetFromFactory(localizationFactory, context);
					ctx.Heap.SetValue(DefaultConfigurationSteps.ContextKeys.Localization, localization);
				}));

			return configurator;
		}

		public static IAppConfigurator SetupCustomLogger([NotNull] this IAppConfigurator configurator, [NotNull] Func<IFrameworkLogger> loggerFactory)
		{
			return configurator.SetupCustomLogger<object>(null, context => loggerFactory());
		}
		
		public static IAppConfigurator SetupCustomLogger<T>([NotNull] this IAppConfigurator configurator, T context, [NotNull] Func<T, IFrameworkLogger> loggerFactory)
		{
			if (configurator == null) throw new ArgumentNullException(nameof(configurator));
			if (loggerFactory == null) throw new ArgumentNullException(nameof(loggerFactory));

			var initializationPhase = configurator.GetStep(DefaultConfigurationSteps.Initialization);
			initializationPhase.AddOrReplace(new SimplePipelineAction(
				DefaultConfigurationSteps.InitializationActions.Logging,
				true,
				ctx =>
				{
					var logger = GetFromFactory(loggerFactory, context);
					ctx.Heap.SetValue(DefaultConfigurationSteps.ContextKeys.Logger, logger);
				}));

			return configurator;
		}

		public static IAppConfigurator SetupCustomIoc([NotNull] this IAppConfigurator configurator, [NotNull] Func<IServiceContainerFactory> serviceContainerFactoryCreator)
		{
			return configurator.SetupCustomIoc<object>(null, context => serviceContainerFactoryCreator());
		}
		
		public static IAppConfigurator SetupCustomIoc<T>([NotNull] this IAppConfigurator configurator, T context, [NotNull] Func<T, IServiceContainerFactory> serviceContainerFactoryCreator)
		{
			if (configurator == null) throw new ArgumentNullException(nameof(configurator));
			if (serviceContainerFactoryCreator == null) throw new ArgumentNullException(nameof(serviceContainerFactoryCreator));

			var initializationPhase = configurator.GetStep(DefaultConfigurationSteps.Initialization);
			initializationPhase.AddOrReplace(new SimplePipelineAction(
				DefaultConfigurationSteps.InitializationActions.Ioc,
				true,
				ctx =>
				{
					var serviceContainerFactory = GetFromFactory(serviceContainerFactoryCreator, context);
					ctx.Heap.SetValue(DefaultConfigurationSteps.ContextKeys.Ioc, serviceContainerFactory);
				}));

			return configurator;
		}

		public static IAppConfigurator RegisterServices([NotNull] this IAppConfigurator configurator, [NotNull] Action<IServiceRegistrator> registerAction)
		{
			return configurator.RegisterServices<object>(null, (context, registrator) => registerAction(registrator));
		}
		
		public static IAppConfigurator RegisterServices<T>([NotNull] this IAppConfigurator configurator, T context, [NotNull] Action<T, IServiceRegistrator> registerAction)
		{
			if (configurator == null) throw new ArgumentNullException(nameof(configurator));
			if (registerAction == null) throw new ArgumentNullException(nameof(registerAction));

			var initializationPhase = configurator.GetStep(DefaultConfigurationSteps.ExternalRegistration);
			initializationPhase.AddAction(new SimplePipelineAction(
				$"{DefaultConfigurationSteps.ExternalRegistrationActions.Registration}_{initializationPhase.Actions.Count}",
				true,
				ctx =>
				{
					var serviceRegistrator = GetObjectFromContext<IServiceRegistrator>(ctx, DefaultConfigurationSteps.ContextKeys.Container);
					registerAction.Invoke(context, serviceRegistrator);
				}));

			return configurator;
		}

	    [NotNull]
	    public static PipelineStep GetStep([NotNull] this IAppConfigurator configurator, string stepName)
	    {
	        var step = configurator.FindStep(stepName);
	        if (step == null)
	            throw new AppConstructingException(Strings.Exceptions.Constructing.StepNotFound, stepName);

	        return step;
	    }

	    public static void InsertBefore([NotNull] this IAppConfigurator configurator, PipelineStep step, PipelineStep insertingStep)
	    {
	        var foundStep = configurator.GetStep(step.Name);
	        var index = configurator.ConfigurationPipeline.Steps.IndexOf(foundStep);
	        configurator.ConfigurationPipeline.Steps.Insert(index, insertingStep);
	    }

	    public static void InsertAfter([NotNull] this IAppConfigurator configurator, PipelineStep step, PipelineStep insertingStep)
	    {
	        var foundStep = configurator.GetStep(step.Name);
	        var index = configurator.ConfigurationPipeline.Steps.IndexOf(foundStep);
	        var insertedIndex = index + 1;
	        if (insertedIndex >= configurator.ConfigurationPipeline.Steps.Count)
	            configurator.ConfigurationPipeline.Steps.Add(insertingStep);
	        else
	            configurator.ConfigurationPipeline.Steps.Insert(index, insertingStep);
	    }

	    [CanBeNull]
	    public static PipelineStep FindStep([NotNull] this IAppConfigurator configurator, string stepName)
	    {
	        return configurator.ConfigurationPipeline.Steps.FindByName(stepName);
	    }

        [NotNull]
		private static T GetFromFactory<T>([NotNull] Func<T> factory)
		{
			if (factory == null) throw new ArgumentNullException(nameof(factory));

			var result = factory.Invoke();
			if (result == null)
				throw new AppConstructingException(Strings.Exceptions.Constructing.FactoryObjectNull);

			return result;
		}
		
		[NotNull]
		private static T GetFromFactory<C,T>([NotNull] Func<C,T> factory, C context)
		{
			if (factory == null) throw new ArgumentNullException(nameof(factory));

			var result = factory.Invoke(context);
			if (result == null)
				throw new AppConstructingException(Strings.Exceptions.Constructing.FactoryObjectNull);

			return result;
		}

		[NotNull]
		private static T GetObjectFromContext<T>(IPipelineContext context, string key) where T : class
		{
			return context.Heap.GetObject<T>(key) ?? throw new AppConstructingException(
			           string.Format(Strings.Exceptions.Constructing.ObjectInContextNotFound, key, typeof(T).Name));
		}
	}
}
