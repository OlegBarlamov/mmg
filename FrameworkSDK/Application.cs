using System;
using System.Collections.Generic;
using FrameworkSDK.Common;
using FrameworkSDK.Constructing;
using FrameworkSDK.Game;
using FrameworkSDK.Game.Mapping;
using FrameworkSDK.Game.Mapping.Default;
using FrameworkSDK.Game.Scenes;
using FrameworkSDK.IoC;
using FrameworkSDK.IoC.Default;
using FrameworkSDK.Localization;
using FrameworkSDK.Logging;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using NetExtensions;

namespace FrameworkSDK
{
	public abstract class Application : IApplication, IDisposable
	{
		[NotNull] protected StartParameters StartParameters { get; } = new StartParameters();

        [NotNull] private LocalizationShell Localization { get; }
		[NotNull] private LoggerShell LoggerShell { get; }
		[NotNull] private GameShell GameShell { get; }
		[NotNull] private ModuleLogger Logger { get; }

		[NotNull, ItemNotNull] private readonly List<ISubsystem> _registeredSubsystems = new List<ISubsystem>();
		[NotNull, ItemNotNull] private readonly List<ISubsystem> _subsystems = new List<ISubsystem>();

		Scene IApplication.CurrentScene => GetCurrentScene();

		[CanBeNull] private IServiceLocator _serviceLocator;

		public Application()
		{
            Localization = new LocalizationShell();
		    LoggerShell = new LoggerShell();
			GameShell = new GameShell(this, LoggerShell);
			Logger = new ModuleLogger(LoggerShell, FrameworkLogModule.Application);
		}
		
        //TODO Refactor. Сейчас компоненты не могут быть созданы в конструкторе приложения.
		public void Run()
		{
			IServiceLocator serviceLocator;

			try
		    {
			    using (var serviceContainerShell = new ServiceContainerShell())
			    {
				    SetupLocalization();
				    SetupLogSystem();
				    SetupIoC(serviceContainerShell);

				    Logger.Info(Strings.Info.DefaultServices);
					RegisterCoreServices(serviceContainerShell);

					Logger.Info(Strings.Info.ConstructingStart);
				    using (var constructor = new AppConstructor(this, serviceContainerShell))
				    {
					    Construct(constructor);
				    }

					GameShell.SetupParameters(StartParameters);

					serviceLocator = BuildContainer(serviceContainerShell);
				    GameShell.ResolveDependencies(serviceLocator);
				    AppSingletone.Initialize(LoggerShell, serviceLocator);
				}

			    Logger.Info(Strings.Info.ConstructingEnd);
				InitializeSubsystems(_registeredSubsystems);
		    }
		    catch (Exception e)
		    {
                throw new FrameworkException(Strings.Exceptions.AppInitialization, e);
		    }

		    Start(serviceLocator);
        }

	    [CanBeNull]
        protected virtual ILocalization GetLocalization()
	    {
	        return null;
	    }

        [CanBeNull]
	    protected virtual IFrameworkLogger GetFrameworkLogger()
	    {
	        return null;
	    }

	    [CanBeNull]
        protected virtual IFrameworkServiceContainer CreateServiceContainer()
	    {
	        return null;
	    }

		protected virtual void Update(GameTime gameTime)
		{
			
		}

		[CanBeNull]
		protected abstract Scene GetCurrentScene();

	    protected abstract void Construct([NotNull] IAppConstructor constructor);

	    private void SetupLocalization()
	    {
	        Localization.SetupLocalization(GetLocalization());
	        Strings.Localization = Localization;
        }

	    private void SetupLogSystem()
	    {
	        LoggerShell.SetupLogger(GetFrameworkLogger());

            if (LoggerShell.IsUsedCustomLogger)
                Logger.Info(Strings.Info.LogRegistered);
	        if (Localization.IsUsedCustomLocalization)
	            Logger.Info(Strings.Info.LocalizationRegistered);
        }

	    private void SetupIoC([NotNull] ServiceContainerShell serviceContainerShell)
		{
			if (serviceContainerShell == null) throw new ArgumentNullException(nameof(serviceContainerShell));

			serviceContainerShell.SetupServiceContainer(CreateServiceContainer());
	        Logger.Info(Strings.Info.IoCRegistered);
        }

		private void RegisterCoreServices(IServiceRegistrator serviceRegistrator)
		{
			serviceRegistrator.RegisterInstance<ILocalization>(Localization);
			serviceRegistrator.RegisterInstance<IFrameworkLogger>(Logger);

			serviceRegistrator.RegisterInstance<IScenesController>(new ScenesController(LoggerShell));
			serviceRegistrator.RegisterInstance<IRandomService>(new DefaultRandomService(new Random(Guid.NewGuid().GetHashCode())));

		    var mappingHost = new MappingHost(CreateServiceContainer() ?? new DefaultServiceContainer(),
		        AppDomain.CurrentDomain.GetAllTypes());
            serviceRegistrator.RegisterInstance<IViewResolver>(mappingHost);
		    serviceRegistrator.RegisterInstance<IControllerResolver>(mappingHost);

            GameShell.RegisterServices(serviceRegistrator);
		}

	    [NotNull] private IServiceLocator BuildContainer([NotNull] ServiceContainerShell serviceContainerShell)
	    {
		    if (serviceContainerShell == null) throw new ArgumentNullException(nameof(serviceContainerShell));

		    Logger.Info(Strings.Info.BuildContainer);
			return serviceContainerShell.BuildContainer();
	    }

        private void InitializeSubsystems(IReadOnlyList<ISubsystem> subsystems)
	    {
            Logger.Info(string.Format(Strings.Info.SubsystemsFound, subsystems.Count));

	        foreach (var subsystem in subsystems)
	        {
	            Logger.Info(string.Format(Strings.Info.SubsystemInitialize, subsystem.Name));

                try
	            {
	                subsystem.Initialize();
		            _subsystems.Add(subsystem);
				}
	            catch (Exception e)
	            {
	                Logger.Info(string.Format(Strings.Exceptions.SubsystemInitializeException, e));
                }
	        }
	    }

	    private void Start([NotNull] IServiceLocator serviceLocator)
	    {
		    _serviceLocator = serviceLocator ?? throw new ArgumentNullException(nameof(serviceLocator));
			Logger.Info(Strings.Info.AppRunning);

		    GameShell.Subsystems.AddRange(_subsystems);

		    try
		    {
			    GameShell.Run();
		    }
		    catch (Exception e)
		    {
			    throw new FrameworkException(Strings.Exceptions.FatalException, e);
		    }
		    finally
		    {
			    Stop();
		    }
        }

		private void Stop()
		{
			GameShell.Stop();
            
		}

		void IApplication.RegisterSubsystem([NotNull] ISubsystem subsystem)
	    {
	        if (subsystem == null) throw new ArgumentNullException(nameof(subsystem));

            _registeredSubsystems.Add(subsystem);
	    }

		void IUpdatable.Update(GameTime gameTime)
		{
			Update(gameTime);
		}

		public void Dispose()
		{
			GameShell.Dispose();

			foreach (var subsystem in _subsystems)
				subsystem.Dispose();

			_serviceLocator?.Dispose();
		}
	}
}
