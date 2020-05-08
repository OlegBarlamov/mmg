using System;
using Console.Core;
using FrameworkSDK.MonoGame.InputManagement;
using FrameworkSDK.MonoGame.Mvc;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Console.FrameworkAdapter
{
    [UsedImplicitly]
    public class DefaultConsoleManipulator : Controller
    {
        private IConsoleController ConsoleController { get; }
        private IInputService InputService { get; }

        public DefaultConsoleManipulator([NotNull] IConsoleController consoleController, [NotNull] IInputService inputService)
        {
            ConsoleController = consoleController ?? throw new ArgumentNullException(nameof(consoleController));
            InputService = inputService ?? throw new ArgumentNullException(nameof(inputService));
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (InputService.Keyboard.KeyPressedOnce(Keys.OemTilde))
            {
                if (ConsoleController.IsShowed)
                    ConsoleController.Hide();
                else
                    ConsoleController.Show();
            }
        }
    }
}