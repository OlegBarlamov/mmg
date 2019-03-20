using System;
using Autofac;
using GameSDK.TraceSystem;
using JetBrains.Annotations;
using Logging;
using Microsoft.Extensions.Logging;

namespace GameSDK
{
	//TODO Remove
	public class ApplicationBase
	{
		internal GameBase Game { get; set; }
	}

	public class Application<TGame> : ApplicationBase, IDisposable where TGame : GameBase
	{
		[NotNull] private ApplicationSettings ApplicationSettings { get; }

		[CanBeNull] private ILogger _appLogger;
		[CanBeNull] private IContainer _serviceLocator;

		private readonly TraceSubSystem _traceSubSystem;

		public Application([NotNull] ApplicationSettings applicationSettings)
		{
			ApplicationSettings = applicationSettings ?? throw new ArgumentNullException(nameof(applicationSettings));

			var traceSettings = new TraceSettings
			{
				LogDirectoryPath = applicationSettings.LogDirectoryPath,
				IsConsoleEnabled = true,
				IsConsoleShowed = false,
				IsDebug = true
			};

			_traceSubSystem = new TraceSubSystem(traceSettings);
		}

		public void Start()
		{
			Start<EmptyModule>();
		}

		public void Start<TModule>() where TModule : IServicesModule
		{
			PreInit();

			_serviceLocator = RegisterServices(typeof(TModule));

			Initialize(_serviceLocator);

			Run();
		}

		public void Dispose()
		{
			_appLogger?.Info("Disposing app...");

			Game.Dispose();

			_appLogger?.Info("Disposing completed");
			_traceSubSystem.Dispose();
		}

		private void PreInit()
		{
			_traceSubSystem.Initialize();

			var loggerFactory = _traceSubSystem.GetLoggerFactory();
			_appLogger = loggerFactory.CreateLogger("app");
			_appLogger?.Info("log system initialized");

			_appLogger?.Info("Pre initialize completed");
		}

		private IContainer RegisterServices(Type moduleType)
		{
			var builder = new ContainerBuilder();
			_appLogger?.Info("Register services...");

			_appLogger?.Info("Register system services...");
			builder.RegisterInstance(_traceSubSystem.GetLoggerFactory()).As<ILoggerFactory>().SingleInstance();
			builder.RegisterInstance(ApplicationSettings).As<ApplicationSettings>().SingleInstance();
			builder.RegisterType(moduleType).Named<IServicesModule>("main_module").SingleInstance();
			var container = builder.Build();
			builder = new ContainerBuilder();

			_appLogger?.Info("Register sdk services...");
			var sdkModule = new SdkServicesModule(this);
			sdkModule.RegisterServices(builder);

			_appLogger?.Info($"Register input module {moduleType.Name}...");
			var module = container.ResolveNamed<IServicesModule>("main_module");

			module.RegisterServices(builder);

			builder.RegisterType<TGame>().As<TGame>().SingleInstance();
			builder.RegisterInstance(container).As<IContainer>().SingleInstance();

			_appLogger?.Info("Register services completed");

			builder.Update(container);
			return container;
		}

		private void Initialize(IContainer serviceLocator)
		{
			_appLogger?.Info("Game Initializing...");

			Game = serviceLocator.Resolve<TGame>();

			_appLogger?.Info("Game Initializing completed");
		}

		private void Run()
		{
			_appLogger?.Info("Running the game...");

			OnGameStarting();
			try
			{
				Game.Run();
			}
			finally
			{
				OnGameStopping();
				Game.Dispose();
			}

			_serviceLocator?.Dispose();
			_appLogger?.Info("Game has been stopped");
		}

		private void OnGameStarting()
		{
			Game.Activated += GameOnActivated;
			Game.Deactivated += GameOnDeactivated;
		}

		private void OnGameStopping()
		{
			Game.Activated -= GameOnActivated;
			Game.Deactivated -= GameOnDeactivated;
		}

		private void GameOnDeactivated(object sender, EventArgs e)
		{
			_appLogger.Info("game deactivated");
			_traceSubSystem.OnDeactivated(Game);
		}

		private void GameOnActivated(object sender, EventArgs e)
		{
			_appLogger.Info("game activated");
			_traceSubSystem.OnActivated(Game);
		}
	}
}
