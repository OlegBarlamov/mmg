using System;
using Console.GameFrameworkAdapter;
using FrameworkSDK.MonoGame;
using FrameworkSDK.MonoGame.InputManagement;
using FrameworkSDK.MonoGame.Mvc;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;

namespace Omegas.Client.MacOs
{
    public class OmegasGameApp : GameApp
    {
        public IInputService InputService { get; }
        public DefaultConsoleManipulator ConsoleManipulator { get; }
        
        protected override SceneBase CurrentScene { get; }

        public OmegasGameApp(
            [NotNull] IInputService inputService,
            [NotNull] DefaultConsoleManipulator consoleManipulator,
            MainScene mainScene,
            MenuScene menuScene)
        {
            InputService = inputService ?? throw new ArgumentNullException(nameof(inputService));
            ConsoleManipulator = consoleManipulator ?? throw new ArgumentNullException(nameof(consoleManipulator));
            CurrentScene = menuScene;
        }

        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            
            ConsoleManipulator.Update(gameTime);
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();

            //InputService.GamePads.ActivateEmulator(PlayerIndex.One, new SimpleKeyboardGamepadEmulator(InputService.Keyboard));
            InputService.GamePads.EnableGamePads();
        }
    }
}