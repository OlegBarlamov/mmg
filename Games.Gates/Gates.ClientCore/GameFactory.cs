using System;
using System.IO;
using FrameworkSDK;
using FrameworkSDK.IoC;
using FrameworkSDK.Constructing;
using FrameworkSDK.Logging;
using FrameworkSDK.MonoGame.Constructing;
using Gates.ClientCore.Logging;
using Logging;
using Microsoft.Extensions.Logging;

namespace Gates.ClientCore
{
    public static class GameFactory
    {
	    private static LogSystem _logSystem;

		public static void Create(IServicesModule services)
        {
            var factory = new AppFactory();
            factory.CreateGame<GameHost>()
				.SetupCustomLogger(CreateLoggerFactory)
				.RegisterServices(x => RegisterServices(x, services))
                .Configure()
                .Run();

	        using (_logSystem)
	        {
		        _logSystem.OpenErrorsLogIfNeed();
	        }
        }

	    private static IFrameworkLogger CreateLoggerFactory()
	    {
		    var logDirectoryRoot = Path.Combine(Environment.CurrentDirectory, "Logs");
		    _logSystem = new LogSystem(logDirectoryRoot, true);
			return new FrameworkLoggerIntegration(_logSystem);
	    }

	    private static void RegisterServices(IServiceRegistrator registrator, IServicesModule services)
        {
			registrator.RegisterInstance<ILoggerFactory>(_logSystem);
            registrator.RegisterModule(new GameCoreModule());
            registrator.RegisterModule(services);
        }
    }
}
