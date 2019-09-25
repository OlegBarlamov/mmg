using System;
using FrameworkSDK.Localization;
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
			Logger = new ModuleLogger(coreLogger, FrameworkLogModule.Scenes);
		}

		public void Update(GameTime gameTime)
		{
			if (_newScene != null)
			{
				ProcessScenesChanging(CurrentScene, _newScene, gameTime);
				return;
			}

		    CurrentScene.Update(gameTime);
		}

		public void Draw(GameTime gameTime)
		{
		    CurrentScene.Draw(gameTime);
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

			var closeState = oldScene.UpdateState(gameTime);
			if (closeState.CanClose)
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

		private void SwitchScene([NotNull] IScene newScene)
		{
			if (newScene == null) throw new ArgumentNullException(nameof(newScene));

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

			_newScene = newScene;
		}

		public void Dispose()
		{
			Logger.Dispose();
		}
	}
}
