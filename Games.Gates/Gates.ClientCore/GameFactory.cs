using System;
using System.IO;
using FrameworkSDK;
using FrameworkSDK.Constructing;
using FrameworkSDK.IoC;
using FrameworkSDK.MonoGame.Constructing;
using Gates.ClientCore.Logging;
using JetBrains.Annotations;
using Logging;
using Microsoft.Extensions.Logging;

namespace Gates.ClientCore
{
    public static class GameFactory
    {
	    public static void Create(IServicesModule services)
		{
			var logger = CreateLoggerFactory();
			var logFactory = new FrameworkLoggerIntegration(logger);
			var module = new Module(logger);
            var factory = new DefaultAppFactory();
            factory.UseLogger(logFactory)
	            .AddServices(module)
	            .AddServices<GameCoreModule>()
	            .AddServices(services)
	            .UseGame<GameHost>()
	            .Construct()
	            .Run();

	        using (logger)
	        {
		        logger.OpenErrorsLogIfNeed();
	        }
        }

	    private static LogSystem CreateLoggerFactory()
	    {
		    var logDirectoryRoot = Path.Combine(Environment.CurrentDirectory, "Logs");
		    return new LogSystem(logDirectoryRoot, true);
	    }
	    
	    private class Module : IServicesModule
	    {
		    public ILoggerFactory LoggerFactory { get; }

		    public Module([NotNull] ILoggerFactory loggerFactory)
		    {
			    LoggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
		    }
		    public void RegisterServices(IServiceRegistrator registrator)
		    {
			    registrator.RegisterInstance(LoggerFactory);
		    }
	    }
    }
}
