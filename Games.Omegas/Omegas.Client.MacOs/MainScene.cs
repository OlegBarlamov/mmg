using System;
using FrameworkSDK.Common;
using FrameworkSDK.MonoGame.Basic;
using FrameworkSDK.MonoGame.Graphics.Camera2D;
using FrameworkSDK.MonoGame.Graphics.DrawableComponents;
using FrameworkSDK.MonoGame.Graphics.DrawableComponents.Stencils;
using FrameworkSDK.MonoGame.Graphics.GraphicsPipeline;
using FrameworkSDK.MonoGame.Graphics.RenderableComponents.Models;
using FrameworkSDK.MonoGame.InputManagement;
using FrameworkSDK.MonoGame.Map;
using FrameworkSDK.MonoGame.Mvc;
using FrameworkSDK.MonoGame.SceneComponents.Controllers;
using FrameworkSDK.MonoGame.Services;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using NetExtensions.Geometry;
using Omegas.Client.MacOs.Models;

namespace Omegas.Client.MacOs
{
    public class MainScene : Scene
    {
        public IInputService InputService { get; }
        public IRandomService RandomService { get; }
        public GameResourcePackage GameResourcePackage { get; }
        public ICamera2DService Camera2DService { get; }
        public IDisplayService DisplayService { get; }
        private CharacterData CharacterData { get; } = new CharacterData();
        
        private Tiled2DMap _map;
        private TiledMapDrawableComponent _mapDrawableComponent;
        
        public MainScene([NotNull] IInputService inputService, [NotNull] IRandomService randomService,
            [NotNull] GameResourcePackage gameResourcePackage, [NotNull] ICamera2DService camera2DService,
            [NotNull] IDisplayService displayService)
            : base("MainScene")
        {
            InputService = inputService ?? throw new ArgumentNullException(nameof(inputService));
            RandomService = randomService ?? throw new ArgumentNullException(nameof(randomService));
            GameResourcePackage = gameResourcePackage ?? throw new ArgumentNullException(nameof(gameResourcePackage));
            Camera2DService = camera2DService ?? throw new ArgumentNullException(nameof(camera2DService));
            DisplayService = displayService ?? throw new ArgumentNullException(nameof(displayService));
        }

        protected override void Initialize()
        {
            var mapData = new Tiled2DCell[10, 10];
            for (int x = 0; x < 10; x++)
            {
                for (int y = 0; y < 10; y++)
                {
                    var texture = GameResourcePackage.MapBackgroundTexturesList.PickRandom();
                    mapData[x, y] = new Tiled2DCell(new Point(x, y), new TextureStencil(texture, Color.White));
                }
            }

            _map = new Tiled2DMap(mapData, new Vector2(256));

            _mapDrawableComponent = (TiledMapDrawableComponent) AddView(new ViewModel<Tiled2DMap>(_map));

            var camera = new SimpleCamera2D(new SizeInt(DisplayService.PreferredBackBufferWidth,
                DisplayService.PreferredBackBufferHeight));
            Camera2DService.SetActiveCamera(camera);

            AddController(new CenteredCameraController(CharacterData, camera));
            AddController(new KeyboardObject2DPositioningController(InputService, CharacterData,
                new KeyboardPositioningController.KeysMap()));
            //AddController(new KeyboardCamera2DController(InputService, camera, new KeyboardCamera2DController.KeysMap()));
            
            AddController(CharacterData);
        }

        protected override IGraphicsPipeline BuildGraphicsPipeline(IGraphicsPipelineBuilder graphicsPipelineBuilder)
        {
            return graphicsPipelineBuilder.Drawing2DPreset(Color.CornflowerBlue, new BeginDrawConfig()).Build();
        }
    }
}