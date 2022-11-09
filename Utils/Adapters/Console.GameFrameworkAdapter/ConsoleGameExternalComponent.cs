using System;
using Console.Core;
using Console.InGame;
using FrameworkSDK.MonoGame.Core;
using FrameworkSDK.MonoGame.ExternalComponents;
using FrameworkSDK.MonoGame.Resources;
using FrameworkSDK.MonoGame.Services;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;

namespace Console.GameFrameworkAdapter
{
    [UsedImplicitly]
    public class ConsoleGameExternalComponent : IExternalGameComponent
    {
        [NotNull] private IConsoleResourcePackage ConsoleResourcePackage { get; }
        [NotNull] private IResourcesService ResourcesService { get; }
        [NotNull] private IGameHeartServices GameHeartServices { get; }
        [NotNull] private IDisplayService DisplayService { get; }
        [NotNull] private InGameConsoleController InGameConsoleController { get; }
        
        public ConsoleGameExternalComponent(
            [NotNull] IConsoleController consoleController,
            [NotNull] IConsoleResourcePackage consoleResourcePackage,
            [NotNull] IResourcesService resourcesService,
            [NotNull] IGameHeartServices gameHeartServices,
            [NotNull] IDisplayService displayService)
        {
            ConsoleResourcePackage = consoleResourcePackage ?? throw new ArgumentNullException(nameof(consoleResourcePackage));
            ResourcesService = resourcesService ?? throw new ArgumentNullException(nameof(resourcesService));
            GameHeartServices = gameHeartServices ?? throw new ArgumentNullException(nameof(gameHeartServices));
            DisplayService = displayService ?? throw new ArgumentNullException(nameof(displayService));
            InGameConsoleController = (InGameConsoleController)consoleController;
        }
        
        public void LoadContent()
        {
            ResourcesService.LoadPackage(ConsoleResourcePackage);
        }
        
        public void Initialize()
        {
            var consoleConfig = new InGameConsoleConfig
            {
                DefaultWidth = DisplayService.PreferredBackBufferWidth,
                Background = ConsoleResourcePackage.Background,
                HeaderBackground = ConsoleResourcePackage.HeaderBackground,
                SuggestSelection = ConsoleResourcePackage.SuggestSelection,
                CommandLineCorner = ConsoleResourcePackage.CommandLineCorner,
                ConsoleFont = ConsoleResourcePackage.ConsoleFont
            };
            InGameConsoleController.Initialize(consoleConfig, GameHeartServices.GraphicsDeviceManager.GraphicsDevice);
        }
        
        public void Update(GameTime gameTime)
        {
            InGameConsoleController.Update(gameTime);
        }

        public void Draw(GameTime gameTime)
        {
            InGameConsoleController.Draw(gameTime, GameHeartServices.SpriteBatch);
        }
        
        public void UnloadContent()
        {
            ResourcesService.UnloadPackage(ConsoleResourcePackage);
        }

        public void Dispose()
        {
            ConsoleResourcePackage.Dispose();
            InGameConsoleController.Dispose();
        }
    }
}