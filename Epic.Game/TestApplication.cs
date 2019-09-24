using System;
using Epic.Battle.Models;
using Epic.Core.Services;
using FrameworkSDK.MonoGame.GameStructure.Scenes;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using NetExtensions;

namespace Epic.Game
{
    internal class TestApplication : FrameworkSDK.MonoGame.Application
    {        
        public override Scene CurrentScene { get; }

        private IConsoleService ConsoleService { get; }

        public TestApplication([NotNull] IConsoleService consoleService)
        {
            ConsoleService = consoleService ?? throw new ArgumentNullException(nameof(consoleService));
            var model = BattleModel.Generate(new Int32Size(5, 5));
            CurrentScene = new BattleScene(model);

            ConsoleService.Show();
        }

        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }
    }
}
