using Atom.Client.Services;
using Atom.Client.Services.Implementations;
using FrameworkSDK;
using FrameworkSDK.Constructing;
using FrameworkSDK.IoC;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Atom.Client
{
	public abstract class ConsoleApp : IAppSubSystem
	{
		protected IConsoleService Console { get; }

		protected ConsoleApp(IConsoleService console)
		{
			Console = console;
			console.Show();
		}

		public abstract void Run();

		public static IAppFactory Create<TConsoleApp>() where TConsoleApp : ConsoleApp
		{
			var loggerFactory = new NullLogFactory();
			var appFactory = new DefaultAppFactory();
			return appFactory.AddServices
				(registrator =>
				{
					registrator.RegisterInstance<ILoggerFactory>(loggerFactory);
					registrator.RegisterType<IConsoleService, ConsoleService>();
				})
				.AddComponent<TConsoleApp>();
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

		public void Dispose()
		{
			
		}

		public void Configure()
		{
			
		}
	}
}
