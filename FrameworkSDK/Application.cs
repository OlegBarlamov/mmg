using System;
using System.Collections.Generic;
using FrameworkSDK.Constructing;
using FrameworkSDK.IoC;
using FrameworkSDK.Localization;
using FrameworkSDK.Logging;
using JetBrains.Annotations;

namespace FrameworkSDK
{
	public abstract class Application : IApplication
	{
        [NotNull] private LocalizationShell Localization { get; }
		[NotNull] private LoggerShell LoggerShell { get; }
		[NotNull] private ModuleLogger Logger { get; }

		[NotNull, ItemNotNull] private readonly List<ISubsystem> _registeredSubsystems = new List<ISubsystem>();

        public Application()
		{
            Localization = new LocalizationShell();
		    LoggerShell = new LoggerShell();
		    Logger = new ModuleLogger(LoggerShell, FrameworkLogModule.Application);
        }
		
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

					Logger.Info(Strings.Info.ConstructingStart);
				    using (var constructor = new AppConstructor(this, serviceContainerShell))
				    {
					    Construct(constructor);
				    }

					serviceLocator = BuildContainer(serviceContainerShell);
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
        protected virtual IFrameworkServiceContainer GetServiceContainer()
	    {
	        return null;
	    }

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

			serviceContainerShell.SetupServiceContainer(GetServiceContainer());
	        Logger.Info(Strings.Info.IoCRegistered);
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
                }
	            catch (Exception e)
	            {
	                Logger.Info(string.Format(Strings.Exceptions.SubsystemInitializeException, e));
                }
	        }
	    }

	    private void Start(IServiceLocator serviceLocator)
	    {

        }

	    void IApplication.RegisterSubsystem([NotNull] ISubsystem subsystem)
	    {
	        if (subsystem == null) throw new ArgumentNullException(nameof(subsystem));

            _registeredSubsystems.Add(subsystem);
	    }
	}
}
