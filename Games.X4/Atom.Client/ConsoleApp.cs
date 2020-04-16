using Atom.Client.Services;
using Atom.Client.Services.Implementations;
using FrameworkSDK;
using FrameworkSDK.Constructing;
using FrameworkSDK.IoC;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Atom.Client
{
	public abstract class ConsoleApp : IApplication
	{
		protected IConsoleService Console { get; }

		protected ConsoleApp(IConsoleService console)
		{
			Console = console;
			console.Show();
		}

		public abstract void Run();

		public static IAppRunner Create<TConsoleApp>() where TConsoleApp : ConsoleApp
		{
			var loggerFactory = new NullLogFactory();
			var appFactory = new AppFactory();
			return appFactory.Create<TConsoleApp>()
				.RegisterServices(registrator =>
				{
					registrator.RegisterInstance<ILoggerFactory>(loggerFactory);
					registrator.RegisterType<IConsoleService, ConsoleService>();
				})
				.Configure();

		}

		private class NullLogFactory : ILoggerFactory
		{
			public void Dispose()
			{
			}

			public ILogger CreateLogger(string categoryName)
			{
				return NullLogger.Instance;
			}

			public void AddProvider(ILoggerProvider provider)
			{
			}
		}
	}
}
