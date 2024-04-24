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
using NetExtensions.Collections;
using NetExtensions.Geometry;
using Omegas.Client.MacOs.Models;
using SimplePhysics2D;
using SimplePhysics2D.Spaces;

namespace Omegas.Client.MacOs
{
    public class MainScene : Scene
    {
        public IInputService InputService { get; }
        public IRandomService RandomService { get; }
        public GameResourcePackage GameResourcePackage { get; }
        public ICamera2DService Camera2DService { get; }
        public IDisplayService DisplayService { get; }
        private CharacterData CharacterData { get; } = new CharacterData(Color.Red);
        private CharacterData CharacterData2 { get; } = new CharacterData(Color.Blue);
        
        private Tiled2DMap _map;
        private TiledMapDrawableComponent _mapDrawableComponent;
        
        public MainScene([NotNull] IInputService inputService, [NotNull] IRandomService randomService,
            [NotNull] GameResourcePackage gameResourcePackage, [NotNull] ICamera2DService camera2DService,
            [NotNull] IDisplayService displayService, [NotNull] IPhysics2DFactory physics2DFactory)
            : base("MainScene")
        {
            if (physics2DFactory == null) throw new ArgumentNullException(nameof(physics2DFactory));
            InputService = inputService ?? throw new ArgumentNullException(nameof(inputService));
            RandomService = randomService ?? throw new ArgumentNullException(nameof(randomService));
            GameResourcePackage = gameResourcePackage ?? throw new ArgumentNullException(nameof(gameResourcePackage));
            Camera2DService = camera2DService ?? throw new ArgumentNullException(nameof(camera2DService));
            DisplayService = displayService ?? throw new ArgumentNullException(nameof(displayService));
            
            UsePhysics2D(physics2DFactory.Create(new HeapCollidersSpace2D()));
        }

        protected override void Initialize()
        {
            var mapData = new Tiled2DCell[10, 10];
            mapData.Fill((x, y) =>
            {
                var texture = GameResourcePackage.MapBackgroundTexturesList.PickRandom();
                return new Tiled2DCell(new Point(x, y), new TextureStencil(texture, Color.White));
            });

            _map = new Tiled2DMap(mapData, new Vector2(256));

            _mapDrawableComponent = (TiledMapDrawableComponent) AddView(new ViewModel<Tiled2DMap>(_map));
            
            CharacterData.SetPosition(new Vector2(200, 200));
            CharacterData2.SetPosition(new Vector2(800, 200));
            CharacterData2.PlayerIndex = PlayerIndex.Two;

            var camera = new SimpleCamera2D(new SizeInt(DisplayService.PreferredBackBufferWidth,
                DisplayService.PreferredBackBufferHeight));
            Camera2DService.SetActiveCamera(camera);

            AddController(new CenteredCameraController(CharacterData, camera));
            // AddController(new KeyboardObject2DPositioningController(InputService, CharacterData,
            //     new KeyboardPositioningController.KeysMap()));
            //AddController(new KeyboardCamera2DController(InputService, camera, new KeyboardCamera2DController.KeysMap()));
            
            AddController(CharacterData);
            AddController(CharacterData2);
            
            Physics2D.ApplyImpulse(CharacterData2, new Vector2(-20f, 0));
        }

        protected override IGraphicsPipeline BuildGraphicsPipeline(IGraphicsPipelineBuilder graphicsPipelineBuilder)
        {
            return graphicsPipelineBuilder.Drawing2DPreset(Color.CornflowerBlue, new BeginDrawConfig()).Build();
        }
    }
}