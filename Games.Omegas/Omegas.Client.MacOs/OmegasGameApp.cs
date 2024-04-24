using System;
using Console.GameFrameworkAdapter;
using FrameworkSDK.MonoGame;
using FrameworkSDK.MonoGame.InputManagement;
using FrameworkSDK.MonoGame.InputManagement.Emulators;
using FrameworkSDK.MonoGame.Map;
using FrameworkSDK.MonoGame.Mvc;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using NetExtensions.Collections;
using NetExtensions.Helpers;

namespace Omegas.Client.MacOs
{
    public class OmegasGameApp : GameApp
    {
        public IInputService InputService { get; }
        public DefaultConsoleManipulator ConsoleManipulator { get; }

        public OmegasGameApp(
            [NotNull] IInputService inputService,
            [NotNull] DefaultConsoleManipulator consoleManipulator,
            MainScene mainScene)
        {
            InputService = inputService ?? throw new ArgumentNullException(nameof(inputService));
            ConsoleManipulator = consoleManipulator ?? throw new ArgumentNullException(nameof(consoleManipulator));
            CurrentScene = mainScene;
        }
        protected override SceneBase CurrentScene { get; }

        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            
            ConsoleManipulator.Update(gameTime);
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();

            InputService.GamePads.ActivateEmulator(PlayerIndex.One, new SimpleKeyboardGamepadEmulator(InputService.Keyboard));
            InputService.GamePads.EnableGamePads();
        }
    }
}