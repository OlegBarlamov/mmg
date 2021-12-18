using FrameworkSDK.Pipelines;
using FrameworkSDK.IoC;
using FrameworkSDK.IoC.Default;
using FrameworkSDK.Localization;
using FrameworkSDK.Logging;
using JetBrains.Annotations;
using NetExtensions;
using NetExtensions.Collections;

// ReSharper disable once CheckNamespace
namespace FrameworkSDK.Constructing
{
    public class DefaultConfigurationFactory
    {
        public Pipeline Create()
        {
            var configuration = new Pipeline();

	        var initializePhase = CreateInitializePhase();
	        var baseSetup = CreateBaseSetupPhase();
	        var registration = CreateRegistrationPhase();
            var externalRegistration = CreateExternalRegistrationPhase();
            var constructing = CreateConstructingPhase();

			configuration.Steps.AddRange(
				initializePhase,
				baseSetup,
				registration,
                externalRegistration,
				constructing);

			return configuration;
        }

	    private PipelineStep CreateInitializePhase()
	    {
			var initializePhase = new PipelineStep(DefaultConfigurationSteps.Initialization);
		    initializePhase.AddActions(
			    new SimplePipelineAction(
				    DefaultConfigurationSteps.InitializationActions.Localization, true,
				    context =>
				    {
					    var localization = new DefaultLocalization();
					    context.Heap.SetValue(DefaultConfigurationSteps.ContextKeys.Localization, localization);
				    }),
			    new SimplePipelineAction(
				    DefaultConfigurationSteps.InitializationActions.Logging, true,
				    context =>
				    {
					    var logger = new NullLogger();
					    context.Heap.SetValue(DefaultConfigurationSteps.ContextKeys.Logger, logger);
					}),
			    new SimplePipelineAction(
				    DefaultConfigurationSteps.InitializationActions.Ioc, true,
				    context =>
				    {
					    var serviceContainerFactory = new DefaultServiceContainerFactory();
					    context.Heap.SetValue(DefaultConfigurationSteps.ContextKeys.Ioc, serviceContainerFactory);
				    })
		    );
		    return initializePhase;
	    }

	    private PipelineStep CreateBaseSetupPhase()
	    {
			var baseSetupPhase = new PipelineStep(DefaultConfigurationSteps.BaseSetup);
		    baseSetupPhase.AddActions(
			    new SimplePipelineAction(
				    DefaultConfigurationSteps.BaseSetupActions.Setup, true,
				    context =>
				    {
					    Strings.Localization = GetObjectFromContext<ILocalization>(context, DefaultConfigurationSteps.ContextKeys.Localization);

					    var logger = GetObjectFromContext<IFrameworkLogger>(context, DefaultConfigurationSteps.ContextKeys.Logger);
						var serviceContainerFactory = GetObjectFromContext<IServiceContainerFactory>(context, DefaultConfigurationSteps.ContextKeys.Ioc);
						var moduleLogger = new ModuleLogger(logger, FrameworkLogModule.Application);
				        var mainServiceContainer = serviceContainerFactory.CreateContainer("main");

					    context.Heap.SetValue(DefaultConfigurationSteps.ContextKeys.Logger, moduleLogger);
						context.Heap.SetValue(DefaultConfigurationSteps.ContextKeys.Ioc, serviceContainerFactory);
				        context.Heap.SetValue(DefaultConfigurationSteps.ContextKeys.Container, mainServiceContainer);
                        context.Heap.SetValue(DefaultConfigurationSteps.ContextKeys.BaseLogger, logger);
					})
		    );
		    return baseSetupPhase;
		}

	    private PipelineStep CreateRegistrationPhase()
	    {
		    var registrationPhase = new PipelineStep(DefaultConfigurationSteps.Registration);
		    registrationPhase.AddActions(
			    new SimplePipelineAction(
				    DefaultConfigurationSteps.RegistrationActions.Core, true,
				    context =>
				    {
					    var logger = context.Heap.GetObject<ModuleLogger>(DefaultConfigurationSteps.ContextKeys.Logger);
						logger?.Info(Strings.Info.AppConstructing.DefaultServices);

					    var localization = GetObjectFromContext<ILocalization>(context, DefaultConfigurationSteps.ContextKeys.Localization);
					    var loggerService = GetObjectFromContext<IFrameworkLogger>(context, DefaultConfigurationSteps.ContextKeys.BaseLogger);
					    var serviceContainer = GetObjectFromContext<IFrameworkServiceContainer>(context, DefaultConfigurationSteps.ContextKeys.Container);
				        var serviceContainerFactory = GetObjectFromContext<IServiceContainerFactory>(context, DefaultConfigurationSteps.ContextKeys.Ioc);

                        var coreModule = new CoreModule(localization, loggerService, serviceContainer, serviceContainerFactory);
				        serviceContainer.RegisterModule(coreModule);
					})
			);
		    return registrationPhase;
	    }

        private PipelineStep CreateExternalRegistrationPhase()
        {
            return new PipelineStep(DefaultConfigurationSteps.ExternalRegistration);
        }


        private PipelineStep CreateConstructingPhase()
	    {
			var registrationPhase = new PipelineStep(DefaultConfigurationSteps.Constructing);
		    registrationPhase.AddActions(
			    new SimplePipelineAction(
				    DefaultConfigurationSteps.ConstructingActions.Core, true,
				    context =>
				    {
					    var serviceContainer = GetObjectFromContext<IFrameworkServiceContainer>(context, DefaultConfigurationSteps.ContextKeys.Container);
						var loggerService = GetObjectFromContext<IFrameworkLogger>(context, DefaultConfigurationSteps.ContextKeys.BaseLogger);

					    var serviceLocator = serviceContainer.BuildContainer();
						context.Heap.SetValue(DefaultConfigurationSteps.ContextKeys.Locator, serviceLocator);

						AppContext.Initialize(loggerService, serviceLocator);
					})
		    );
		    return registrationPhase;
		}

		[NotNull]
	    private static T GetObjectFromContext<T>(IPipelineContext context, string key) where T : class
	    {
			return context.Heap.GetObject<T>(key) ?? throw new AppConstructingException(
			           string.Format(Strings.Exceptions.Constructing.ObjectInContextNotFound, key, typeof(T).Name));
		}
	}
}
