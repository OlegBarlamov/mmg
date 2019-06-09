using System;
using FrameworkSDK.Localization;
using FrameworkSDK.Logging;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;

namespace FrameworkSDK.Game.Scenes
{
	internal class ScenesController : IScenesController
	{
		public bool CanSceneChanged { get; private set; }

		public Scene CurrentScene
		{
			get => GetCurrentScene();
			set => SwitchScene(value);
		}

		[CanBeNull] private Scene _newScene;
		[CanBeNull] private Scene _previousClosingScene;
		[NotNull] private Scene _currentScene;

		private readonly object _sceneChangingLocker = new object();

		private ModuleLogger Logger { get; }

		public ScenesController([NotNull] Scene defaultScene, [NotNull] IFrameworkLogger coreLogger)
		{
			if (coreLogger == null) throw new ArgumentNullException(nameof(coreLogger));
			_currentScene = defaultScene ?? throw new ArgumentNullException(nameof(defaultScene));

			Logger = new ModuleLogger(coreLogger, FrameworkLogModule.Scenes);
		}

		public void Update(GameTime gameTime)
		{
			if (_newScene != null)
			{
				ProcessScenesChanging(_currentScene, _newScene, gameTime);
				return;
			}

			_currentScene.Update(gameTime);
		}

		public void Draw(GameTime gameTime)
		{
			_currentScene.Draw(gameTime);
		}

		private Scene GetCurrentScene()
		{
			return _currentScene;
		}

		private void ProcessScenesChanging([NotNull] IClosable oldScene, [NotNull] Scene newScene, GameTime gameTime)
		{
			if (oldScene == null) throw new ArgumentNullException(nameof(oldScene));
			if (newScene == null) throw new ArgumentNullException(nameof(newScene));

			if (_previousClosingScene != newScene)
			{
				_previousClosingScene = newScene;
				Logger.Info(Strings.Info.SceneSwitchingState, oldScene, newScene);
			}

			var closeState = oldScene.Update(gameTime);
			if (closeState.CanClose)
				CloseAndSwitchScenes(oldScene, newScene);
		}

		private void CloseAndSwitchScenes([NotNull] IClosable oldScene, [NotNull] Scene newScene)
		{
			oldScene.OnClosed();
			_currentScene = newScene;
			_newScene = null;
			newScene.OnOpened();
			Logger.Info(Strings.Info.SceneSwitched, oldScene, newScene);
		}

		private void SwitchScene([NotNull] Scene newScene)
		{
			if (newScene == null) throw new ArgumentNullException(nameof(newScene));

			lock (_sceneChangingLocker)
			{
				if (newScene == _currentScene)
					return;

				if (!CanSceneChanged)
				{
					Logger.Error(Strings.Errors.SceneChangingWhileNotAllowed, newScene);
					return;
				}

				CanSceneChanged = false;
			}

			_newScene = newScene;
		}

		public void Dispose()
		{
			Logger.Dispose();
		}
	}
}
