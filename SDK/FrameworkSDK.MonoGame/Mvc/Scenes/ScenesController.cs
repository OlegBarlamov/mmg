using System;
using FrameworkSDK.MonoGame.Localization;
using FrameworkSDK.Logging;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;

// ReSharper disable once CheckNamespace
namespace FrameworkSDK.MonoGame.Mvc
{
    [UsedImplicitly]
	internal class ScenesController : IScenesController
	{
	    public bool CanSceneChange { get; private set; } = true;

		public IScene CurrentScene
		{
			get => GetCurrentScene();
			set => SwitchScene(value);
		}

	    private static IScene DefaultScene => _defaultScene ?? (_defaultScene = new EmptyScene());
	    private static IScene _defaultScene;

		[CanBeNull] private IScene _newScene;
		[CanBeNull] private IScene _previousClosingScene;
		[CanBeNull] private IScene _currentScene;

		private readonly object _sceneChangingLocker = new object();

		private ModuleLogger Logger { get; }

		public ScenesController([NotNull] IFrameworkLogger coreLogger)
		{
			if (coreLogger == null) throw new ArgumentNullException(nameof(coreLogger));
			Logger = new ModuleLogger(coreLogger, LogCategories.Mvc);
		}

		public void Update(GameTime gameTime)
		{
			var currentScene = CurrentScene;
			if (_newScene != null)
			{
				try
				{
					ProcessScenesChanging(currentScene, _newScene, gameTime);
				}
				catch (Exception e)
				{
					Logger.Error("Error during scenes changing: {0}->{1}", e, currentScene, _newScene);
				}
			}

			try
			{
				currentScene.Update(gameTime);
			}
			catch (Exception e)
			{
				Logger.Error("Scene {0} Update unhandled exception", e, currentScene);
			}
		}

		private IScene GetCurrentScene()
		{
			return _currentScene ?? DefaultScene;
		}

		private void ProcessScenesChanging([NotNull] IClosable oldScene, [NotNull] IScene newScene, GameTime gameTime)
		{
			if (oldScene == null) throw new ArgumentNullException(nameof(oldScene));
			if (newScene == null) throw new ArgumentNullException(nameof(newScene));

			if (_previousClosingScene != newScene)
			{
				_previousClosingScene = newScene;
				Logger.Info(Strings.Info.SceneSwitchingState, oldScene, newScene);
			}
			
			if (oldScene.ReadyToBeClosed)
				CloseAndSwitchScenes(oldScene, newScene);
		}

		private void CloseAndSwitchScenes([NotNull] IClosable oldScene, [NotNull] IScene newScene)
		{
			oldScene.OnClosed();
			_currentScene = newScene;
			_newScene = null;
			newScene.OnOpened();
		    CanSceneChange = true;
			Logger.Info(Strings.Info.SceneSwitched, oldScene, newScene);
		}

		private void SwitchScene(IScene newScene)
		{
			lock (_sceneChangingLocker)
			{
				if (newScene == _currentScene)
					return;

				if (!CanSceneChange)
				{
					Logger.Error(Strings.Errors.SceneChangingWhileNotAllowed, newScene);
					return;
				}

				CanSceneChange = false;
			}

			_currentScene?.CloseRequest();
			newScene.OnOpening();
			_newScene = newScene;
		}

		public void Dispose()
		{
			Logger.Dispose();
		}
	}
}
