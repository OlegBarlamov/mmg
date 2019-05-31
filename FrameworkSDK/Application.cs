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
		private readonly GameShell _gameShell;
        private readonly List<ISubsystem> _registeredSubsystems = new List<ISubsystem>();

        private LocalizationShell Localization { get; }
        private LoggerShell LoggerShell { get; }
	    private ServiceContainerShell ServiceContainerShell { get; }

        private ModuleLogger Logger { get; }

        public Application()
		{
			_gameShell = new GameShell();
            Localization = new LocalizationShell();
		    LoggerShell = new LoggerShell();
		    Logger = new ModuleLogger(LoggerShell, FrameworkLogModule.Application);
		    ServiceContainerShell = new ServiceContainerShell();
        }
		
		public void Run()
		{
		    try
		    {
                SetupLocalization();
                SetupLogSystem();
                SetupIoC();
                
		        using (var constructor = new AppConstructor(this))
		        {
		            Construct(constructor);
		        }

		        InitializeSubsystems(_registeredSubsystems);

		        Start();
		    }
		    catch (Exception e)
		    {
                throw new FrameworkException(Strings.Exceptions.AppInitialization, e);
		    }
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

	    private void SetupIoC()
	    {
            ServiceContainerShell.SetupServiceContainer(GetServiceContainer());
	        Logger.Info(Strings.Info.IoCRegistered);
        }

        private void InitializeSubsystems(IEnumerable<ISubsystem> subsystems)
	    {

	    }

	    private void Start()
	    {
	        _gameShell.Run();
        }

	    void IApplication.RegisterSubsystem([NotNull] ISubsystem subsystem)
	    {
	        if (subsystem == null) throw new ArgumentNullException(nameof(subsystem));

            _registeredSubsystems.Add(subsystem);
	    }
	}
}
