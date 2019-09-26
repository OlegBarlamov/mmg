using System;
using Atom.Client.Logging;
using Atom.Client.Services;
using Atom.Client.Services.Implementations;
using FrameworkSDK;
using FrameworkSDK.Constructing;
using FrameworkSDK.IoC;
using FrameworkSDK.MonoGame.Constructing;
using Microsoft.Extensions.Logging;

namespace Atom.Client
{
#if WINDOWS || LINUX
    /// <summary>
    /// The main class.
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
			var logFactory = new LogFactory("Logs", true);

	        var appFactory = new AppFactory();
	        using (var app = appFactory.CreateGame<AtomApp>()
		        .SetupCustomLogger(logFactory.CreateAdapter)
		        .RegisterServices(registrator =>
		        {
			        registrator.RegisterInstance<ILogFactory>(logFactory);
			        registrator.RegisterInstance<ILoggerFactory>(logFactory);
			        registrator.RegisterType<IConsoleService, ConsoleService>();
				})
		        .Configure())
	        {
		        app.Run();
	        }
		}
    }
#endif
}
